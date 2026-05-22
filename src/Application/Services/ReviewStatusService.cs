using Microsoft.Extensions.Logging;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;


namespace IntegrationImport.Application.Services;

public class ReviewStatusService : IReviewStatusService
{
    private readonly IImportRequestRepository _importRepository;
    private readonly IDbTransactionScope _dbtransactionScope;
    private readonly IErrorCatalog _errors;
    private readonly ILogger<ReviewStatusService> _logger;

    public ReviewStatusService(
        IImportRequestRepository importRepository,
        IDbTransactionScope dbtransactionScope,
        IErrorCatalog errors,
        ILogger<ReviewStatusService> logger)
    {
        _importRepository = importRepository;
        _dbtransactionScope = dbtransactionScope;
        _errors = errors;
        _logger = logger;
    }

    public async Task<Result<ImportStatusResponseDto>?> GetStatusAsync(Guid syncId, CancellationToken ct)
    {
        var sync = await _importRepository.GetTrackedByIdWithItemsAsync(syncId, ct);

        if (sync is null)
            return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.ImportRequestNotFound);

        var items = ResolveItems(sync);

        return Result<ImportStatusResponseDto>.Ok(BuildStatusResponse(sync, items));
    }

    public async Task<Result<ImportStatusResponseDto>> ReviewItemsAsync(ReviewImportRequestDto request, CancellationToken ct)
    {
        if (request.SyncId == Guid.Empty)
            return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.SyncIdRequired);

        if (request.Items is null || request.Items.Count == 0)
            return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.AtLeastOneItemReviewDecisionRequired);

        if (ValidateUniqueItemDecisions(request))
            return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.DuplicateItemDecisionsDetected);

        var importRequest = await _importRepository.GetTrackedByIdWithItemsAsync(request.SyncId, ct);
        if (importRequest is null)
            return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.ImportRequestNotFound);

        var items = ResolveItems(importRequest);

        var reviewer = string.IsNullOrWhiteSpace(request.ReviewedBy)
            ? "system"
            : request.ReviewedBy.Trim();

        foreach (var impItem in request.Items)
        {
            IImportItem? item = items.FirstOrDefault(x => x.Id == impItem.ItemId);

            if (item is null)
            {
                _logger.LogError($"Item '{impItem.ItemId}' does not belong to sync '{request.SyncId}'.");
                return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.ItemDoesNotBelongToSync);
            }

            if (item.ReviewStatus != ImportItemReviewStatus.PENDING_REVIEW)
            {
                _logger.LogError($"Item '{impItem.ItemId}' is not in pending review status. Current status: {item.ReviewStatus}");
                return _errors.Fail<ImportStatusResponseDto>(ErrorCodes.IMPORT.ItemNotInPendingReviewStatus);
            }

            item.ReviewedBy    = reviewer;
            item.ReviewedAtUtc = DateTime.UtcNow;
            item.ReviewNotes   = impItem.Notes;

            switch (impItem.Decision)
            {
                case ReviewDecision.APPROVED:
                    item.ReviewStatus  = ImportItemReviewStatus.APPROVED;
                    item.ProcessStatus = ImportItemProcessStatus.PENDING_DISPATCH;
                    break;
                case ReviewDecision.REJECTED:
                    item.ReviewStatus  = ImportItemReviewStatus.REJECTED;
                    item.ProcessStatus = ImportItemProcessStatus.SKIPPED;
                    break;
                default:
                    throw new InvalidOperationException("Invalid review decision.");
            }
        }

        RecalculateBatchStatus(importRequest, items);

        await _dbtransactionScope.SaveChangesAsync(ct);

        return Result<ImportStatusResponseDto>.Ok(BuildStatusResponse(importRequest, items));
    }

    /// <summary>
    /// Returns the right collection (Users or OrganizationUnits) based on the
    /// EntityType of the import request, exposed as IImportItem so the rest of
    /// the service is collection-agnostic.
    /// </summary>
    private static IReadOnlyList<IImportItem> ResolveItems(ImportRequest sync) =>
        sync.EntityType switch
        {
            ImportEntityType.USER              => sync.Users.Cast<IImportItem>().ToList(),
            ImportEntityType.ORGANIZATION_UNIT => sync.OrganizationUnits.Cast<IImportItem>().ToList(),
            _                                  => Array.Empty<IImportItem>(),
        };

    private bool ValidateUniqueItemDecisions(ReviewImportRequestDto request)
    {
        var duplicateItemIds = request.Items
            .GroupBy(x => x.ItemId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        var duplicates = duplicateItemIds.Count > 0;

        if (duplicates)
            _logger.LogError($"Duplicate item decisions detected in request. ItemIds: {string.Join(", ", duplicateItemIds)}");

        return duplicates;
    }

    private void RecalculateBatchStatus(ImportRequest importRequest, IReadOnlyList<IImportItem> items)
    {
        if (items.Count == 0)
        {
            importRequest.Status = ImportJobStatus.PENDING_REVIEW;
            importRequest.ReviewedAtUtc = null;
            return;
        }

        var pendingReviewExists = items.Any(x => x.ReviewStatus == ImportItemReviewStatus.PENDING_REVIEW);
        var reviewedExists      = items.Any(x => x.ReviewStatus != ImportItemReviewStatus.PENDING_REVIEW);

        if (!reviewedExists)
        {
            importRequest.Status = ImportJobStatus.PENDING_REVIEW;
            importRequest.ReviewedAtUtc = null;
            return;
        }

        if (pendingReviewExists)
        {
            importRequest.Status = ImportJobStatus.UNDER_REVIEW;
            importRequest.ReviewedAtUtc = null;
            return;
        }

        importRequest.Status = ImportJobStatus.REVIEW_COMPLETED;

        if (!importRequest.ReviewedAtUtc.HasValue)
            importRequest.ReviewedAtUtc = DateTime.UtcNow;
    }

    private ImportStatusResponseDto BuildStatusResponse(ImportRequest sync, IReadOnlyList<IImportItem> items)
    {
        return new ImportStatusResponseDto
        {
            SyncId       = sync.Id,
            ReferenceId  = sync.ReferenceId,
            Status       = sync.Status.ToString(),
            ReceivedAt   = sync.ReceivedAtUtc,
            ProcessedAt  = sync.ReviewedAtUtc ?? sync.CompletedAtUtc,
            ItemCount    = sync.ItemCount,

            PendingReviewItemCount = items.Count(x => x.ReviewStatus == ImportItemReviewStatus.PENDING_REVIEW),
            ApprovedItemCount      = items.Count(x => x.ReviewStatus == ImportItemReviewStatus.APPROVED),
            RejectedItemCount      = items.Count(x => x.ReviewStatus == ImportItemReviewStatus.REJECTED),

            DispatchedItemCount    = items.Count(x => x.ProcessStatus == ImportItemProcessStatus.DISPATCHED),
            AppliedItemCount       = items.Count(x => x.ProcessStatus == ImportItemProcessStatus.APPLIED),
            FailedItemCount        = items.Count(x => x.ProcessStatus == ImportItemProcessStatus.FAILED),

            Notes = sync.Notes,

            Items = items.Select(x => new ImportStatusItemDto
            {
                ItemId        = x.Id,
                HrmsId        = x.HrmsId,
                ReviewStatus  = x.ReviewStatus.ToString(),
                ProcessStatus = x.ProcessStatus.ToString(),
                Notes         = x.ReviewNotes
            }).ToList()
        };
    }
}
