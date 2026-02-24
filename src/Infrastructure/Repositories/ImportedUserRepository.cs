using ExternalIntegrations.OrganizationChart.Domain.Entities;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;
using ExternalIntegrations.OrganizationChart.Infrastructure.Database;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Repositories;

public class ImportedUserRepository : IImportedUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImportedUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ImportedUser?> GetByExternalIdAsync(string externalId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ImportedUser>> GetByPartnerCodeAsync(string partnerCode)
    {
        throw new NotImplementedException();
    }

    public Task<List<ImportedUser>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ImportedUser importedUser)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ImportedUser importedUser)
    {
        throw new NotImplementedException();
    }
}
