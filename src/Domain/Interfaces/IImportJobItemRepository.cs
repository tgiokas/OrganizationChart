using ExternalIntegrations.OrganizationChart.Domain.Entities;
using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Domain.Interfaces;

public interface IImportJobItemRepository
{
    Task<List<ImportJobItem>> GetByJobIdAsync(int importJobId);
    Task<List<ImportJobItem>> GetByJobIdAndStatusAsync(int importJobId, ImportJobItemStatus status);
    Task AddRangeAsync(List<ImportJobItem> items);
    Task UpdateAsync(ImportJobItem item);
}
