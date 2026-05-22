using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;


namespace IntegrationImport.Infrastructure.Repositories;

public class ImportedOrganizationUnitRepository : IImportedOrganizationUnitRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImportedOrganizationUnitRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ImportedOrganizationUnit?> GetByExternalIdAsync(string externalId, CancellationToken ct)
    {
        return _dbContext.ImportedOrganizationUnits
            .FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);
    }

    public async Task<List<ImportedOrganizationUnit>> GetByExternalIdsAsync(IEnumerable<string> externalIds, CancellationToken ct)
    {
        var idList = externalIds.ToList();
        if (idList.Count == 0) return [];

        return await _dbContext.ImportedOrganizationUnits
            .Where(x => idList.Contains(x.ExternalId))
            .ToListAsync(ct);
    }

    public Task<ImportedOrganizationUnit?> GetBySourceImportItemIdAsync(Guid sourceImportItemId, CancellationToken ct)
    {
        return _dbContext.ImportedOrganizationUnits
            .FirstOrDefaultAsync(x => x.SourceImportItemId == sourceImportItemId, ct);
    }

    public async Task AddAsync(ImportedOrganizationUnit entity, CancellationToken ct)
    {
        await _dbContext.ImportedOrganizationUnits.AddAsync(entity, ct);
    }
}
