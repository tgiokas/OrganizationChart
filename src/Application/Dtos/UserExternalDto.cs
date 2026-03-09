namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class UserExternalDto
{
    public required string ExternalId { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public Dictionary<string, string>? Attributes { get; set; }
}
