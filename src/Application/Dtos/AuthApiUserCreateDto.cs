namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

/// <summary>
/// DTO sent to DMS.Authentication API to create a user.
/// Should match the UserCreateDto in DMS.Authentication.
/// </summary>
public class AuthApiUserCreateDto
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
