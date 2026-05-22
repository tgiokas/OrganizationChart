using IntegrationImport.Application.Dtos;
using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Application.Interfaces;

public interface IOrgUnitImportMapper
{
    ImportRequest MapToImportRequest(OrgUnitImportRequestDto request, IReadOnlyList<OrgUnitItemDto> acceptedOrgUnits);
    void ApplyInitialStatuses(ImportRequest entity, bool requireAdminApproval);
}
