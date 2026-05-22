using System.Text.Json;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Application.Services;

public class UserImportMapper : IUserImportMapper
{
    public ImportRequest MapToSyncRequest(UsersImportRequestDto request, IReadOnlyList<UserItemDto> acceptedUsers)
    {
        Guid syncId = Guid.NewGuid();

        return new ImportRequest
        {
            Id            = syncId,
            ReferenceId   = request.ReferenceId,
            EntityType    = ImportEntityType.USER,
            EffectiveDate = request.EffectiveDate,
            Status        = ImportJobStatus.PENDING_REVIEW,
            ReceivedAtUtc = DateTime.UtcNow,
            ItemCount     = acceptedUsers.Count,
            Users = acceptedUsers.Select(x => new UserImportItem
            {
                Id                  = Guid.NewGuid(),
                ImportRequestId     = syncId,
                HrmsId              = x.HrmsId,
                Action              = x.Action.Trim().ToUpperInvariant(),
                Username            = x.Username,
                FirstName           = x.FirstName,
                LastName            = x.LastName,
                Email               = x.Email,
                PhoneNumber         = x.PhoneNumber,
                Enabled             = x.Enabled,
                VatId               = x.VatId,
                ReviewStatus        = ImportItemReviewStatus.PENDING_REVIEW,
                ProcessStatus       = ImportItemProcessStatus.PENDING_DISPATCH,
                ChangedFieldsJson   = x.ChangedFields is { Count: > 0 }
                                          ? JsonSerializer.Serialize(x.ChangedFields)
                                          : null,
                OrgUnitHrmsIdsJson  = x.OrgUnitHrmsIds is { Count: > 0 }
                                          ? JsonSerializer.Serialize(x.OrgUnitHrmsIds)
                                          : null
            }).ToList()
        };
    }
}
