using Microsoft.EntityFrameworkCore;

using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;

namespace IntegrationImport.Infrastructure.Repositories;

public class ImportRequestRepository : IImportRequestRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ImportRequestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ImportRequest?> GetTrackedByIdWithUserItemsAsync(Guid syncId, CancellationToken ct)
    {
        return _dbContext.ImportRequests
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == syncId, ct);
    }

    public Task<ImportRequest?> GetTrackedByIdWithItemsAsync(Guid syncId, CancellationToken ct)
    {
        return _dbContext.ImportRequests
            .Include(x => x.Users)
            .Include(x => x.OrganizationUnits)
            .FirstOrDefaultAsync(x => x.Id == syncId, ct);
    }


 
    public Task<bool> ExistsAsync(string referenceId, ImportEntityType entityType, CancellationToken ct)
    {
        return _dbContext.ImportRequests
            .AnyAsync(x => x.ReferenceId == referenceId && x.EntityType == entityType, ct);
    }

    public async Task AddAsync(ImportRequest importRequest, CancellationToken ct)
    {
        await _dbContext.ImportRequests.AddAsync(importRequest, ct);
    }

    public Task<ImportRequest?> GetBySyncIdAsync(Guid syncId, CancellationToken ct)
    {
        return _dbContext.ImportRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == syncId, ct);
    }

    public Task<ImportRequest?> GetTrackedByIdAsync(Guid syncId, CancellationToken ct)
    {
        return _dbContext.ImportRequests
            .FirstOrDefaultAsync(x => x.Id == syncId, ct);
    }

}
