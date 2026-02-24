using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Infrastructure.ApiClients;
using Microsoft.Extensions.Logging;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.ExternalServices;

/// HTTP client that calls Authentication REST API
/// to create, update, and disable users.
public class AuthApiHttpClient : ApiClientBase, IAuthApiClient
{   
    public AuthApiHttpClient(HttpClient httpClient, ILogger<AuthApiHttpClient> logger) 
        : base(httpClient, logger)
    {
        
    }

    public Task<Result<AuthApiUserProfileDto>> CreateUserAsync(AuthApiUserCreateDto request)
    {
        //  POST to Authentication /user/create       
        throw new NotImplementedException();
    }

    public Task<Result<AuthApiUserProfileDto>> UpdateUserAsync(AuthApiUserUpdateDto request)
    {
        //  POST to Authentication /user/update        
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DisableUserAsync(string keycloakUserId)
    {
        //  POST to Authentication /user/diable       
        throw new NotImplementedException();
    }
}
