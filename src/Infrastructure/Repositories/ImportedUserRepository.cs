using Microsoft.EntityFrameworkCore;

using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;
using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Repositories;

public class ImportedUserRepository :  IImportedUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ImportedUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }



    public Task<ImportedUser?> GetBySourceImportItemIdAsync(Guid sourceImportItemId, CancellationToken ct)
    {
        return _dbContext.ImportedUsers
            .FirstOrDefaultAsync(x => x.SourceImportItemId == sourceImportItemId, ct);
    }

    public async Task<ImportedUser?> GetByExternalIdAsync(string externalId, CancellationToken ct)
    {
        return await _dbContext.ImportedUsers
            .FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);
    }

    public async Task<ImportedUser?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct)
    {
        return await _dbContext.ImportedUsers
            .FirstOrDefaultAsync(x => x.KeycloakUserId.ToString() == keycloakUserId, ct);
    }

    public async Task AddAsync(ImportedUser importedUser, CancellationToken ct)
    {
        await _dbContext.ImportedUsers.AddAsync(importedUser, ct);
    }

    public Task<ImportedUser?> GetByExternalIdWithOrgUnitsAsync(string externalId, CancellationToken ct)
    {
        return _dbContext.ImportedUsers
            .Include(x => x.OrganizationUnits)
                .ThenInclude(x => x.ImportedOrganizationUnit)
            .FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);
    }



    //public async Task UpdateAsync(ImportedUser importedUser, CancellationToken ct)
    //{
    //   await _dbContext.ImportedUsers.Update(importedUser);
    //}
}
