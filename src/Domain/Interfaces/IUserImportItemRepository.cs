using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Domain.Interfaces;

public interface IUserImportItemRepository
{
    Task<UserImportItem?> GetTrackedUserItemByIdAsync(Guid itemId, CancellationToken ct);
    Task<List<UserImportItem>> GetApprovedPendingDispatchUserItemsAsync(int batchSize, CancellationToken ct);
}
