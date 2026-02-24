using ExternalIntegrations.OrganizationChart.Domain.Entities;

namespace ExternalIntegrations.OrganizationChart.Domain.Interfaces;

public interface IImportedUserRepository
{
    Task<ImportedUser?> GetByExternalIdAsync(string externalId);
    Task<List<ImportedUser>> GetByPartnerCodeAsync(string partnerCode);
    Task<List<ImportedUser>> GetAllAsync();
    Task AddAsync(ImportedUser importedUser);
    Task UpdateAsync(ImportedUser importedUser);
}
