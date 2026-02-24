namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class AuthApiUserCreateDto
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}