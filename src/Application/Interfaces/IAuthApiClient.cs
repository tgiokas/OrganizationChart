using ExternalIntegrations.OrganizationChart.Application.Dtos;

namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

public interface IAuthApiClient
{
    Task<Result<AuthApiUserProfileDto>> CreateUserAsync(AuthApiUserCreateDto request);

    Task<Result<AuthApiUserProfileDto>> UpdateUserAsync(AuthApiUserUpdateDto request);

    Task<Result<bool>> DisableUserAsync(string keycloakUserId);
}
