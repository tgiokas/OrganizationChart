namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class AuthApiUserUpdateDto
{
    public required string Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
