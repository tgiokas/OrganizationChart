using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Domain.Interfaces;

public interface IImportedOrganizationUnitRepository
{
    Task<ImportedOrganizationUnit?> GetByExternalIdAsync(string externalId, CancellationToken ct);
    Task<List<ImportedOrganizationUnit>> GetByExternalIdsAsync(IEnumerable<string> externalIds, CancellationToken ct);
    Task<ImportedOrganizationUnit?> GetBySourceImportItemIdAsync(Guid sourceImportItemId, CancellationToken ct);
    Task AddAsync(ImportedOrganizationUnit entity, CancellationToken ct);
}
