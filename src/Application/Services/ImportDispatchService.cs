using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using IntegrationImport.Application.Configuration;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Application.Services;

public class ImportDispatchService : IImportDispatchService
{
    private readonly IImportRequestRepository _importRepository;
    private readonly IUserImportItemRepository _userImportItemRepository;
    private readonly IOrganizationUnitImportItemRepository _orgUnitImportItemRepository;
    private readonly IDbTransactionScope _dbtransactionScope;
    private readonly IMessagePublisher _eventPublisher;
    private readonly IOptions<KafkaSettings> _kafkaOptions;
    private readonly ILogger<ImportDispatchService> _logger;
    private readonly IErrorCatalog _errors;

    public ImportDispatchService(
        IImportRequestRepository importRepository,
        IUserImportItemRepository userImportItemRepository,
        IOrganizationUnitImportItemRepository orgUnitImportItemRepository,
        IDbTransactionScope dbtransactionScope,
        IMessagePublisher eventPublisher,
        IOptions<KafkaSettings> kafkaOptions,
        ILogger<ImportDispatchService> logger,
        IErrorCatalog errors)
    {
        _importRepository = importRepository;
        _userImportItemRepository = userImportItemRepository;
        _orgUnitImportItemRepository = orgUnitImportItemRepository;
        _dbtransactionScope = dbtransactionScope;
        _eventPublisher = eventPublisher;
        _kafkaOptions = kafkaOptions;
        _logger = logger;
        _errors = errors;
    }

    public async Task<Result<string>> DispatchApprovedUserItemAsync(Guid itemId, CancellationToken ct)
    {
        var item = await _userImportItemRepository.GetTrackedUserItemByIdAsync(itemId, ct);

        if (item is null)
        {
            _logger.LogError($"User import item with ID {itemId} was not found.");
            return _errors.Fail<string>(ErrorCodes.IMPORT.ItemNotFound);
        }

        if (item.ReviewStatus != ImportItemReviewStatus.APPROVED)
        {
            _logger.LogError($"User import item with ID {itemId} is not approved. Current review status: {item.ReviewStatus}");
             return _errors.Fail<string>(ErrorCodes.IMPORT.ItemNotApproved);
        }

        if (item.ProcessStatus != ImportItemProcessStatus.PENDING_DISPATCH)
        {
            _logger.LogInformation($"User import item with ID {itemId} has current process status: {item.ProcessStatus}");
             return _errors.Fail<string>(ErrorCodes.IMPORT.ItemStatusNotPendingDispatch);
        }

        item.DispatchAttempts++;
        item.LastDispatchError = null;

        await _dbtransactionScope.SaveChangesAsync(ct);

        try
        {
            var message = new ImportΙtemApprovedMessageDto
            {
                SyncId = item.ImportRequestId,
                ItemId = item.Id,
                ReferenceId = item.ImportRequest.ReferenceId,
                EntityType = item.ImportRequest.EntityType,
                Status = item.ReviewStatus.ToString(),
                ApprovedAtUtc = item.ReviewedAtUtc ?? DateTime.UtcNow
            };

            var topic = _kafkaOptions.Value.Topic;

            await _eventPublisher.PublishJsonAsync(topic,item.Id.ToString(),message,null, ct);

            item.ProcessStatus = ImportItemProcessStatus.DISPATCHED;
            item.DispatchedAtUtc = DateTime.UtcNow;
            item.LastDispatchError = null;

            await _dbtransactionScope.SaveChangesAsync(ct);

            return Result<string>.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.InnerException == null ? ex.Message : ex.InnerException.Message);

            item.LastDispatchError = ex.Message;
            await _dbtransactionScope.SaveChangesAsync(ct);

            throw;
        }
    }

    public async Task<Result<string>> DispatchApprovedOrgUnitItemAsync(Guid orgUnitId, CancellationToken ct)
    {
        var item = await _orgUnitImportItemRepository.GetTrackedOrgUnitItemByIdAsync(orgUnitId, ct);

        if (item is null)
        {
            _logger.LogError($"OrgUnit import item with ID {orgUnitId} was not found.");
            return _errors.Fail<string>(ErrorCodes.IMPORT.ItemNotFound);
        }

        if (item.ReviewStatus != ImportItemReviewStatus.APPROVED)
        {
            _logger.LogError($"OrgUnit import item with ID {orgUnitId} is not approved. Current review status: {item.ReviewStatus}");
            return _errors.Fail<string>(ErrorCodes.IMPORT.ItemNotApproved);
        }

        if (item.ProcessStatus != ImportItemProcessStatus.PENDING_DISPATCH)
        {
            _logger.LogInformation($"OrgUnit import item with ID {orgUnitId} has current process status: {item.ProcessStatus}");
            return _errors.Fail<string>(ErrorCodes.IMPORT.ItemStatusNotPendingDispatch);
        }

        item.DispatchAttempts++;
        item.LastDispatchError = null;

        await _dbtransactionScope.SaveChangesAsync(ct);

        try
        {
            var message = new ImportΙtemApprovedMessageDto
            {
                SyncId        = item.ImportRequestId,
                ItemId        = item.Id,
                ReferenceId   = item.ImportRequest.ReferenceId,
                EntityType    = item.ImportRequest.EntityType,
                Status        = item.ReviewStatus.ToString(),
                ApprovedAtUtc = item.ReviewedAtUtc ?? DateTime.UtcNow
            };

            var topic = _kafkaOptions.Value.Topic;

            await _eventPublisher.PublishJsonAsync(topic, item.Id.ToString(), message, null, ct);

            item.ProcessStatus     = ImportItemProcessStatus.DISPATCHED;
            item.DispatchedAtUtc   = DateTime.UtcNow;
            item.LastDispatchError = null;

            await _dbtransactionScope.SaveChangesAsync(ct);

            return Result<string>.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.InnerException == null ? ex.Message : ex.InnerException.Message);

            item.LastDispatchError = ex.Message;
            await _dbtransactionScope.SaveChangesAsync(ct);

            throw;
        }
    }
}
