using IntegrationImport.Application.Dtos;
using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Application.Helpers;

public interface IUserOrgUnitSynchronizer
{
    Task<Result<List<ImportedOrganizationUnit>>> ResolveAsync(UserImportItem item, CancellationToken ct);
    Task<Result<bool>> SyncAsync(UserImportItem item,ImportedUser importedUser,IReadOnlyCollection<string> changedFields,CancellationToken ct);
}