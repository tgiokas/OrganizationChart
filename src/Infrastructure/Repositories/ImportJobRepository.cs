using ExternalIntegrations.OrganizationChart.Domain.Entities;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;
using ExternalIntegrations.OrganizationChart.Infrastructure.Database;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Repositories;

public class ImportJobRepository : IImportJobRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImportJobRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ImportJob?> GetByJobIdAsync(Guid jobId)
    {
        throw new NotImplementedException();
    }

    public Task<ImportJob?> GetByJobIdWithItemsAsync(Guid jobId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ImportJob job)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ImportJob job)
    {
        throw new NotImplementedException();
    }
}
