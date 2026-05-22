using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;

using System.Text.Json;

namespace IntegrationImport.Application.Services;

public class OrgUnitImportMapper : IOrgUnitImportMapper
{
    public ImportRequest MapToImportRequest(OrgUnitImportRequestDto request, IReadOnlyList<OrgUnitItemDto> acceptedOrgUnits)
    {
        var importRequestId = Guid.NewGuid();

        var items = acceptedOrgUnits.Select(ou => new OrganizationUnitImportItem
        {
            Id = Guid.NewGuid(),
            ImportRequestId = importRequestId,
            HrmsId = ou.HrmsId,
            Action = ou.Action.Trim().ToUpperInvariant(),
            Name = ou.Name,
            Abbreviation = ou.Abbreviation,
            ParentHrmsId = ou.ParentHrmsId,
            Email = ou.Email,
            Location = ou.Location,
            IsActive = ou.Active,
            IsVirtual = ou.IsVirtual,
            ChangedFieldsJson = ou.ChangedFields is { Count: > 0 }
                                      ? JsonSerializer.Serialize(ou.ChangedFields)
                                      : null,
        }).ToList();

        return new ImportRequest
        {
            Id = importRequestId,
            ReferenceId = request.ReferenceId,
            EffectiveDate = request.EffectiveDate,
            EntityType = ImportEntityType.ORGANIZATION_UNIT,
            ReceivedAtUtc = DateTime.UtcNow,
            ItemCount = items.Count,
            OrganizationUnits = items,
        };
    }

    public void ApplyInitialStatuses(ImportRequest entity, bool requireAdminApproval)
    {
        if (requireAdminApproval)
        {
            entity.Status = ImportJobStatus.PENDING_REVIEW;
            foreach (var item in entity.OrganizationUnits)
            {
                item.ReviewStatus = ImportItemReviewStatus.PENDING_REVIEW;
                item.ProcessStatus = ImportItemProcessStatus.PENDING_DISPATCH;
            }
        }
        else
        {
            entity.Status = ImportJobStatus.REVIEW_COMPLETED;
            entity.ReviewedAtUtc = DateTime.UtcNow;
            foreach (var item in entity.OrganizationUnits)
            {
                item.ReviewStatus = ImportItemReviewStatus.APPROVED;
                item.ReviewedAtUtc = DateTime.UtcNow;
                item.ProcessStatus = ImportItemProcessStatus.PENDING_DISPATCH;
            }
        }
    }
}
