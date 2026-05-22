namespace IntegrationImport.Application.Dtos;

public class AuthApiUserCreateDto
{
    public required UserCreateDto User { get; set; }
    public required RoleDto Role { get; set; }
}

public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool PasswordTemp { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool Enabled { get; set; }
    public bool IsAdmin { get; set; }
    public bool EmailVerified { get; set; }
}
public class RoleDto
{
    public required string RoleName { get; set; }
    public string? Description { get; set; }
}
