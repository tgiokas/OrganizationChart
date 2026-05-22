using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Infrastructure.ApiClients;

namespace IntegrationImport.Infrastructure.ExternalServices;

public class AuthApiHttpClient : ApiClientBase, IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };



    public AuthApiHttpClient(HttpClient httpClient, ILogger<AuthApiHttpClient> logger, IErrorCatalog errorCatalog)
        : base(httpClient, logger, errorCatalog)
    {
    }


    public async Task<Result<AuthApiUserProfileDto>> GetUserByUsernameAsync(string username, CancellationToken ct)
    {

        if (string.IsNullOrWhiteSpace(username))
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }
            
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/getbyname")
        {
            Content = new StringContent(
            JsonSerializer.Serialize(new { username }),
            Encoding.UTF8,
            "application/json")
        };

        using var response = await SendRequestRetryAsync(httpRequest, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseBody))
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);

        try
        {
            var result = JsonSerializer.Deserialize<Result<AuthApiUserProfileDto>>(responseBody, JsonOptions);
            return result ?? _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }
        catch (JsonException)
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }
    }

    public async Task<Result<AuthApiUserProfileDto>> CreateUserWithRoleAsync(AuthApiUserCreateDto request, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/create-with-role")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await SendRequestRetryAsync(httpRequest, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }

        try
        {
            var result = JsonSerializer.Deserialize<Result<AuthApiUserProfileDto>>(responseBody, JsonOptions);

            if (result is null)
            {
                return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
            }

            return result;
        }
        catch (JsonException)
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        }
    }

    public async Task<Result<AuthApiUserProfileDto>> UpdateUserAsync(AuthApiUserUpdateDto request, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/update")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await SendRequestRetryAsync(httpRequest, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiUpdateFailed);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiUpdateFailed);

        }

        try
        {
            var result = JsonSerializer.Deserialize<Result<AuthApiUserProfileDto>>(responseBody, JsonOptions);

            if (result is null)
            {
                return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiUpdateFailed);
            }

            return result;
        }
        catch (JsonException)
        {
            return _errorCatalog.Fail<AuthApiUserProfileDto>(ErrorCodes.IMPORT.AuthApiUpdateFailed);

        }
    }

    public async Task<Result<bool>> DisableUserAsync(string keycloakUserId, CancellationToken ct)
    {
        var request = new AuthApiUserDisableDto { Id = keycloakUserId };
        var json = JsonSerializer.Serialize(request, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/update")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await SendRequestRetryAsync(httpRequest, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            return _errorCatalog.Fail<bool>(ErrorCodes.IMPORT.AuthApiDisableFailed);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return _errorCatalog.Fail<bool>(ErrorCodes.IMPORT.AuthApiDisableFailed);
        }

        try
        {
            var result = JsonSerializer.Deserialize<Result<AuthApiUserProfileDto>>(responseBody, JsonOptions);

            if (result is null)
            {
                return _errorCatalog.Fail<bool>(ErrorCodes.IMPORT.AuthApiDisableFailed);
            }

            return Result<bool>.Ok(data: true, message: "Users disabled successfully.");
        }
        catch (JsonException)
        {
            return _errorCatalog.Fail<bool>(ErrorCodes.IMPORT.AuthApiDisableFailed);

        }
    }

    public async Task<Result<bool>> AssignDefaultRoleAsync(Guid keycloakUserId, string roleName, CancellationToken ct)
    {
        var body = new { UserId = keycloakUserId, RoleName = roleName };
        var json = JsonSerializer.Serialize(body, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "user/assign-role")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await SendRequestRetryAsync(httpRequest, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            return _errorCatalog.Fail<bool>(ErrorCodes.IMPORT.AuthApiCreateFailed);
        // (consider adding a dedicated ErrorCodes.IMPORT.AuthApiAssignRoleFailed)

        return Result<bool>.Ok(data: true, message: "Role assigned.");
    }
}
