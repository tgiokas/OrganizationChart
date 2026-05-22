using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Domain.Interfaces;

public interface IImportedUserRepository
{
    Task<ImportedUser?> GetByExternalIdAsync(string externalId, CancellationToken ct);
    Task<ImportedUser?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct);  
    Task AddAsync(ImportedUser importedUser, CancellationToken ct);

    Task<ImportedUser?> GetBySourceImportItemIdAsync(Guid sourceImportItemId, CancellationToken ct);
    Task<ImportedUser?> GetByExternalIdWithOrgUnitsAsync(string externalId, CancellationToken ct);

    //Task<List<ImportedUser>> GetAllAsync();
    //Task UpdateAsync(ImportedUser importedUser, CancellationToken ct); 
}
