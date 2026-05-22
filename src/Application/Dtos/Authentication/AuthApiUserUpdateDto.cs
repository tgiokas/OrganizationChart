namespace IntegrationImport.Application.Dtos;

public class AuthApiUserUpdateDto
{
    public string? Id { get; set; }
    //public required string ExternalId { get; set; }
    //public required string KeycloakUserId { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool Enabled { get; set; }
    public bool IsAdmin { get; set; }
    public bool EmailVerified { get; set; }
}
