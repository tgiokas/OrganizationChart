using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IAuthApiClient
{
    Task<Result<AuthApiUserProfileDto>> CreateUserWithRoleAsync(AuthApiUserCreateDto request, CancellationToken ct);
    Task<Result<AuthApiUserProfileDto>> UpdateUserAsync(AuthApiUserUpdateDto request, CancellationToken ct);
    Task<Result<bool>> DisableUserAsync(string keycloakUserId, CancellationToken ct);
    Task<Result<AuthApiUserProfileDto>> GetUserByUsernameAsync(string username, CancellationToken ct);
    Task<Result<bool>> AssignDefaultRoleAsync(Guid keycloakUserId, string roleName, CancellationToken ct);
}
