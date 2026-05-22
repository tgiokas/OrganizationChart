using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IOrgUnitImportService
{
    Task<Result<PartialImportResponseDto>> ImportOrgUnitsAsync(OrgUnitImportRequestDto request, CancellationToken ct);
}
