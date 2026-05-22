using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IImportDispatchService
{
    Task<Result<string>> DispatchApprovedUserItemAsync(Guid userId, CancellationToken ct);

    Task<Result<string>> DispatchApprovedOrgUnitItemAsync(Guid orgUnitId, CancellationToken ct);
}
