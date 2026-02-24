using ExternalIntegrations.OrganizationChart.Domain.Entities;
using ExternalIntegrations.OrganizationChart.Domain.Enums;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;
using ExternalIntegrations.OrganizationChart.Infrastructure.Database;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Repositories;

public class ImportJobItemRepository : IImportJobItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImportJobItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ImportJobItem>> GetByJobIdAsync(int importJobId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ImportJobItem>> GetByJobIdAndStatusAsync(int importJobId, ImportJobItemStatus status)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(List<ImportJobItem> items)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ImportJobItem item)
    {
        throw new NotImplementedException();
    }
}
