using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using IntegrationImport.Application.Configuration;
using IntegrationImport.Application.Constants;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Application.Interfaces.Validation;
using IntegrationImport.Application.Validation;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Application.Services;

public class OrgUnitImportService : IOrgUnitImportService
{
    private readonly IOrgUnitImportValidator _validator;
    private readonly IOrgUnitImportMapper _mapper;
    private readonly IImportRequestRepository _importRequestRepository;
    private readonly IImportedOrganizationUnitRepository _importedOrgUnitRepository;
    private readonly IDbTransactionScope _transactionScope;
    private readonly IntegrationImportSettings _settings;
    private readonly IErrorCatalog _errors;
    private readonly ILogger<OrgUnitImportService> _logger;

    public OrgUnitImportService(
        IOrgUnitImportValidator validator,
        IOrgUnitImportMapper mapper,
        IImportRequestRepository importRequestRepository,
        IImportedOrganizationUnitRepository importedOrgUnitRepository,
        IDbTransactionScope transactionScope,
        IOptions<IntegrationImportSettings> settings,
        IErrorCatalog errors,
        ILogger<OrgUnitImportService> logger)
    {
        _validator = validator;
        _mapper = mapper;
        _importRequestRepository = importRequestRepository;
        _importedOrgUnitRepository = importedOrgUnitRepository;
        _transactionScope = transactionScope;
        _settings = settings.Value;
        _errors = errors;
        _logger = logger;
    }

    public async Task<Result<PartialImportResponseDto>> ImportOrgUnitsAsync(OrgUnitImportRequestDto request, CancellationToken ct)
    {
        // ---- 1. Idempotency: same ReferenceId never re-runs.
        bool exists = await _importRequestRepository.ExistsAsync(
            request.ReferenceId,
            ImportEntityType.ORGANIZATION_UNIT,
            ct);

        if (exists)
        {
            _logger.LogError("An ORGANIZATION_UNIT import with referenceId '{ReferenceId}' already exists.", request.ReferenceId);
            return _errors.Fail<PartialImportResponseDto>(ErrorCodes.IMPORT.OrgUnitImportExists);
        }

        // ---- 2. Collect ALL validation errors (shape + parent existence + cascade) in one structured result.
        ValidationResult validation = _validator.Validate(request);
        await AppendInvalidParentsAsync(request, validation, ct);

        // ---- 3. Global errors doom the whole batch.
        if (validation.HasGlobalErrors)
        {
            _logger.LogWarning(
                "OrgUnit import has batch-level validation errors ({Count}): {Errors}",
                validation.GlobalErrors.Count, string.Join("; ", validation.GlobalErrors));

            return _errors.Fail<PartialImportResponseDto>(
                ErrorCodes.IMPORT.ValidationFailed,
                string.Join("; ", validation.GlobalErrors));
        }

        // ---- 4. Partition by validation outcome.
        var invalidIdx = validation.InvalidIndexes;
        var orgUnits = request.OrganizationUnits!;
        var acceptedOrgUnits = new List<OrgUnitItemDto>(orgUnits.Count - invalidIdx.Count);
        var rejected = new List<RejectedItemDto>(invalidIdx.Count);

        for (var i = 0; i < orgUnits.Count; i++)
        {
            var ou = orgUnits[i];
            if (invalidIdx.Contains(i))
            {
                rejected.Add(new RejectedItemDto
                {
                    Index  = i,
                    HrmsId = ou?.HrmsId,
                    Errors = validation.ItemErrors.TryGetValue(i, out var errs)
                                ? new List<string>(errs)
                                : new List<string>()
                });
            }
            else
            {
                acceptedOrgUnits.Add(ou);
            }
        }

        // ---- 5. If every item failed, nothing is persisted.
        if (acceptedOrgUnits.Count == 0)
        {
            _logger.LogWarning(
                "OrgUnit import '{ReferenceId}' fully rejected at ingest: {RejectedCount} item(s). Details: {@Rejected}",
                request.ReferenceId, rejected.Count, rejected);

            var failResult = _errors.Fail<PartialImportResponseDto>(
                ErrorCodes.IMPORT.AllItemsRejected,
                $"All {rejected.Count} item(s) failed validation.");

            failResult.Data = new PartialImportResponseDto
            {
                ReferenceId    = request.ReferenceId,
                SubmittedCount = orgUnits.Count,
                AcceptedCount  = 0,
                RejectedCount  = rejected.Count,
                Rejected       = rejected,
                ReceivedAt     = DateTime.UtcNow,
                Message        = failResult.Message,
            };
            return failResult;
        }

        // ---- 6. Map only the accepted org units, then persist.
        ImportRequest entity = _mapper.MapToImportRequest(request, acceptedOrgUnits);
        _mapper.ApplyInitialStatuses(entity, _settings.RequireAdminApproval);

        await _importRequestRepository.AddAsync(entity, ct);
        await _transactionScope.SaveChangesAsync(ct);

        if (rejected.Count > 0)
        {
            _logger.LogWarning(
                "OrgUnit import '{ReferenceId}': accepted {Accepted}/{Submitted}, rejected {RejectedCount}. Details: {@Rejected}",
                request.ReferenceId, acceptedOrgUnits.Count, orgUnits.Count, rejected.Count, rejected);
        }

        return Result<PartialImportResponseDto>.Ok(new PartialImportResponseDto
        {
            SyncId         = entity.Id,
            ReferenceId    = entity.ReferenceId,
            Status         = entity.Status.ToString(),
            ReceivedAt     = entity.ReceivedAtUtc,
            SubmittedCount = orgUnits.Count,
            AcceptedCount  = acceptedOrgUnits.Count,
            RejectedCount  = rejected.Count,
            Rejected       = rejected,
            Message        = rejected.Count == 0
                ? (_settings.RequireAdminApproval
                    ? "All items received and are pending review."
                    : "All items received. Items were auto-approved and are pending dispatch.")
                : $"{acceptedOrgUnits.Count} of {orgUnits.Count} org units accepted; {rejected.Count} rejected (see 'rejected' for details)."
        });
    }

    /// <summary>
    /// Validates ParentHrmsId references with strict semantics + cascade rejection:
    ///   (1) Self-reference        -> reject the item.
    ///   (2) Parent in batch & valid -> OK (will be created together).
    ///   (3) Parent in batch but invalid -> cascade reject the child.
    ///   (4) Parent exists in DB -> OK.
    ///   (5) Parent nowhere -> reject "Parent not found".
    /// Re-runs until no new rejections appear, so cascades propagate through
    /// deep hierarchies (A->B->C: if A is invalid, B and C both get rejected).
    /// Items already invalid from shape validation are left as-is.
    /// </summary>
    private async Task AppendInvalidParentsAsync(
        OrgUnitImportRequestDto request,
        ValidationResult validation,
        CancellationToken ct)
    {
        if (request?.OrganizationUnits is null) return;

        var items = request.OrganizationUnits;

        // Collect index -> trimmed parent ref, for items whose parent will actually be touched.
        // Gate aligns ingest validation with runtime semantics:
        //   - CREATE: always validate (parent is part of the new record).
        //   - UPDATE/DEACTIVATE: validate only when "ParentHrmsId" is in changedFields,
        //     since the runtime updater is a no-op for unchanged fields.
        var parentRefs = new Dictionary<int, string>();
        for (var i = 0; i < items.Count; i++)
        {
            var ou = items[i];
            if (ou is null) continue;
            if (string.IsNullOrWhiteSpace(ou.ParentHrmsId)) continue;

            var action = ou.Action?.Trim().ToUpperInvariant();
            var isCreate = action == OrgUnitAction.Create;
            var changedFieldsIncludesParent = ou.ChangedFields is { Count: > 0 }
                && ou.ChangedFields.Contains(OrgUnitChangedField.ParentHrmsId, StringComparer.OrdinalIgnoreCase);

            if (!isCreate && !changedFieldsIncludesParent) continue;

            parentRefs[i] = ou.ParentHrmsId.Trim();
        }

        if (parentRefs.Count == 0) return;

        // Map HrmsId -> index (case-insensitive) for in-batch lookups.
        var batchByHrmsId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < items.Count; i++)
        {
            var hrmsId = items[i]?.HrmsId;
            if (string.IsNullOrWhiteSpace(hrmsId)) continue;
            batchByHrmsId.TryAdd(hrmsId.Trim(), i);
        }

        // One-shot DB lookup for parent refs that aren't in the batch.
        var parentsNotInBatch = parentRefs.Values
            .Where(p => !batchByHrmsId.ContainsKey(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var dbParentSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (parentsNotInBatch.Count > 0)
        {
            var dbHits = await _importedOrgUnitRepository.GetByExternalIdsAsync(parentsNotInBatch, ct);
            foreach (var hit in dbHits) dbParentSet.Add(hit.ExternalId);
        }

        // Iterate until no new invalidations — propagates cascades through depth.
        bool changed;
        do
        {
            changed = false;
            var invalid = validation.InvalidIndexes;

            foreach (var (index, parentRef) in parentRefs)
            {
                if (invalid.Contains(index)) continue; // already rejected

                var selfHrmsId = items[index]?.HrmsId?.Trim();
                if (!string.IsNullOrWhiteSpace(selfHrmsId)
                    && string.Equals(selfHrmsId, parentRef, StringComparison.OrdinalIgnoreCase))
                {
                    validation.AddItem(index, $"ParentHrmsId cannot reference itself ('{parentRef}').");
                    changed = true;
                    continue;
                }

                if (batchByHrmsId.TryGetValue(parentRef, out var parentIdx))
                {
                    if (invalid.Contains(parentIdx))
                    {
                        validation.AddItem(index,
                            $"ParentHrmsId '{parentRef}' references an item rejected in this same batch.");
                        changed = true;
                    }
                    continue;
                }

                if (!dbParentSet.Contains(parentRef))
                {
                    validation.AddItem(index,
                        $"ParentHrmsId '{parentRef}' was not found in the system and is not in this batch.");
                    changed = true;
                }
            }
        } while (changed);
    }
}
