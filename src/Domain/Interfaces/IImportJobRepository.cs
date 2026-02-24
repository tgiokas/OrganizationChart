using ExternalIntegrations.OrganizationChart.Domain.Entities;

namespace ExternalIntegrations.OrganizationChart.Domain.Interfaces;

public interface IImportJobRepository
{
    Task<ImportJob?> GetByJobIdAsync(Guid jobId);
    Task<ImportJob?> GetByJobIdWithItemsAsync(Guid jobId);
    Task AddAsync(ImportJob job);
    Task UpdateAsync(ImportJob job);
}
