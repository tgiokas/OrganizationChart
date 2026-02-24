namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class AuthApiUserResponseDto
{
    public bool Success { get; set; }
    public AuthApiUserProfileDto? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
}

public class AuthApiUserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool? Enabled { get; set; }
}
