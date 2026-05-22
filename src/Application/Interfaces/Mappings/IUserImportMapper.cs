using IntegrationImport.Application.Dtos;
using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Application.Interfaces;

public interface IUserImportMapper
{
   
    ImportRequest MapToSyncRequest(UsersImportRequestDto request, IReadOnlyList<UserItemDto> acceptedUsers);
}
