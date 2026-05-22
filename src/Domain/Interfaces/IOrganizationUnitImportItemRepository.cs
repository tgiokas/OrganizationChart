using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Domain.Interfaces;

public interface IOrganizationUnitImportItemRepository
{
    Task<List<OrganizationUnitImportItem>> GetApprovedPendingDispatchOrgUnitItemsAsync(int batchSize, CancellationToken ct);

    Task<OrganizationUnitImportItem?> GetTrackedOrgUnitItemByIdAsync(Guid itemId, CancellationToken ct);
}
