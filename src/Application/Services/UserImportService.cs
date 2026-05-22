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

public class UserImportService : IUserImportService
{
    private readonly IImportRequestRepository _importRepository;
    private readonly IImportedOrganizationUnitRepository _importedOrganizationUnitRepository;
    private readonly IUserImportMapper _mapper;
    private readonly IUserImportValidator _validator;
    private readonly IDbTransactionScope _dbtransactionScope;
    private readonly IntegrationImportSettings _importSettings;
    private readonly IErrorCatalog _errors;
    private readonly ILogger<UserImportService> _logger;

    public UserImportService(
        IImportRequestRepository importRepository,
        IImportedOrganizationUnitRepository importedOrganizationUnitRepository,
        IUserImportMapper mapper,
        IUserImportValidator validator,
        IDbTransactionScope dbtransactionScope,
        IOptions<IntegrationImportSettings> importSettings,
        IErrorCatalog errors,
        ILogger<UserImportService> logger)
    {
        _importRepository = importRepository;
        _importedOrganizationUnitRepository = importedOrganizationUnitRepository;
        _mapper = mapper;
        _validator = validator;
        _dbtransactionScope = dbtransactionScope;
        _importSettings = importSettings.Value;
        _errors = errors;
        _logger = logger;
    }

    public async Task<Result<PartialImportResponseDto>> ImportUsersAsync(UsersImportRequestDto request, CancellationToken ct)
    {
        // ---- 1. Idempotency: same ReferenceId never re-runs.
        bool exists = await _importRepository.ExistsAsync(
           request.ReferenceId,
           ImportEntityType.USER,
           ct);

        if (exists)
        {
            _logger.LogError($"A USER import with referenceId '{request.ReferenceId}' already exists.");
            return _errors.Fail<PartialImportResponseDto>(ErrorCodes.IMPORT.UserImportExists);
        }

        // ---- 2. Collect ALL validation errors (shape + OrgUnit existence) in one structured result.
        ValidationResult validation = _validator.Validate(request);
        await AppendMissingOrgUnitsAsync(request, validation, ct);

        // ---- 3. Global errors doom the whole batch (missing referenceId, batch too large, etc.).
        if (validation.HasGlobalErrors)
        {
            _logger.LogWarning(
                "User import has batch-level validation errors ({Count}): {Errors}",
                validation.GlobalErrors.Count, string.Join("; ", validation.GlobalErrors));

            return _errors.Fail<PartialImportResponseDto>(
                ErrorCodes.IMPORT.ValidationFailed,
                string.Join("; ", validation.GlobalErrors));
        }

        // ---- 4. Partition users by validation outcome.
        var invalidIdx = validation.InvalidIndexes;
        var users = request.Users!;
        var acceptedUsers = new List<UserItemDto>(users.Count - invalidIdx.Count);
        var rejected = new List<RejectedItemDto>(invalidIdx.Count);

        for (var i = 0; i < users.Count; i++)
        {
            var u = users[i];
            if (invalidIdx.Contains(i))
            {
                rejected.Add(new RejectedItemDto
                {
                    Index  = i,
                    HrmsId = u?.HrmsId,
                    Errors = validation.ItemErrors.TryGetValue(i, out var errs)
                                ? new List<string>(errs)
                                : new List<string>()
                });
            }
            else
            {
                acceptedUsers.Add(u);
            }
        }

        // ---- 5. If every user failed, nothing is persisted.
        if (acceptedUsers.Count == 0)
        {
            _logger.LogWarning(
                "User import '{ReferenceId}' fully rejected at ingest: {RejectedCount} item(s). Details: {@Rejected}",
                request.ReferenceId, rejected.Count, rejected);

            var failResult = _errors.Fail<PartialImportResponseDto>(
                ErrorCodes.IMPORT.AllItemsRejected,
                $"All {rejected.Count} item(s) failed validation.");

            // Attach the per-item details so HRMS can see what to fix even on total rejection.
            failResult.Data = new PartialImportResponseDto
            {
                ReferenceId    = request.ReferenceId,
                SubmittedCount = users.Count,
                AcceptedCount  = 0,
                RejectedCount  = rejected.Count,
                Rejected       = rejected,
                ReceivedAt     = DateTime.UtcNow,
                Message        = failResult.Message,
            };
            return failResult;
        }

        // ---- 6. Map only the accepted users, then persist.
        ImportRequest entity = _mapper.MapToSyncRequest(request, acceptedUsers);
        ApplyInitialStatuses(entity);

        await _importRepository.AddAsync(entity, ct);
        await _dbtransactionScope.SaveChangesAsync(ct);

        if (rejected.Count > 0)
        {
            _logger.LogWarning(
                "User import '{ReferenceId}': accepted {Accepted}/{Submitted}, rejected {RejectedCount}. Details: {@Rejected}",
                request.ReferenceId, acceptedUsers.Count, users.Count, rejected.Count, rejected);
        }

        return Result<PartialImportResponseDto>.Ok(new PartialImportResponseDto
        {
            SyncId         = entity.Id,
            ReferenceId    = entity.ReferenceId,
            Status         = entity.Status.ToString(),
            ReceivedAt     = entity.ReceivedAtUtc,
            SubmittedCount = users.Count,
            AcceptedCount  = acceptedUsers.Count,
            RejectedCount  = rejected.Count,
            Rejected       = rejected,
            Message        = rejected.Count == 0
                ? (_importSettings.RequireAdminApproval
                    ? "All items received and are pending review."
                    : "All items received. Items were auto-approved and are pending dispatch.")
                : $"{acceptedUsers.Count} of {users.Count} users accepted; {rejected.Count} rejected (see 'rejected' for details)."
        });
    }

    private void ApplyInitialStatuses(ImportRequest request)
    {
        DateTime now = DateTime.UtcNow;

        if (_importSettings.RequireAdminApproval)
        {
            request.Status = ImportJobStatus.PENDING_REVIEW;

            foreach (var item in request.Users)
            {
                item.ReviewStatus = ImportItemReviewStatus.PENDING_REVIEW;
                item.ProcessStatus = ImportItemProcessStatus.PENDING_DISPATCH;
                item.ReviewedBy = null;
                item.ReviewedAtUtc = null;
                item.ReviewNotes = null;
            }

            return;
        }

        request.Status = ImportJobStatus.REVIEW_COMPLETED;
        request.ReviewedAtUtc = now;

        foreach (var item in request.Users)
        {
            item.ReviewStatus = ImportItemReviewStatus.APPROVED;
            item.ReviewedBy = "system";
            item.ReviewedAtUtc = now;
            item.ReviewNotes = "Auto-approved by configuration.";
            item.ProcessStatus = ImportItemProcessStatus.PENDING_DISPATCH;
        }
    }

    private async Task AppendMissingOrgUnitsAsync(UsersImportRequestDto request, ValidationResult validation, CancellationToken ct)
    {
        if (request?.Users is null) return;

        // Map each user index -> set of OrgUnitHrmsIds it references.
        // Also build the union for a single DB lookup.
        var perUserRefs = new Dictionary<int, List<string>>();
        var allRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < request.Users.Count; i++)
        {
            var user = request.Users[i];
            if (user?.OrgUnitHrmsIds is not { Count: > 0 }) continue;

            // Gate: align ingest validation with runtime sync semantics.
            // For CREATE, OrgUnits are always part of the new record, so always validate.
            // For UPDATE/DEACTIVATE, only validate when "OrgUnitHrmsIds" is in changedFields —
            // mirrors UserOrgUnitSynchronizer.SyncAsync, which is a no-op otherwise.
            var action = user.Action?.Trim().ToUpperInvariant();
            var isCreate = action == UserAction.Create;
            var changedFieldsIncludesOu = user.ChangedFields is { Count: > 0 }
                && user.ChangedFields.Contains(UserChangedField.OrgUnitHrmsIds, StringComparer.OrdinalIgnoreCase);

            if (!isCreate && !changedFieldsIncludesOu) continue;

            var ids = user.OrgUnitHrmsIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ids.Count == 0) continue;

            perUserRefs[i] = ids;
            foreach (var id in ids) allRefs.Add(id);
        }

        if (allRefs.Count == 0) return;

        var existing = await _importedOrganizationUnitRepository
            .GetByExternalIdsAsync(allRefs, ct);

        var existingIds = existing.Select(x => x.ExternalId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Attach each missing-OrgUnit error to the SPECIFIC user that referenced it,
        // so partial-success can exclude only that user and let the rest through.
        foreach (var (index, ids) in perUserRefs)
        {
            foreach (var id in ids.Where(x => !existingIds.Contains(x)))
                validation.AddItem(index, $"OrgUnitHrmsId '{id}' not found in the system.");
        }
    }
}

