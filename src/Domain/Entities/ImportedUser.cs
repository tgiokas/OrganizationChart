namespace ExternalIntegrations.OrganizationChart.Domain.Entities;

/// Mapping table: links the partner's ExternalId to the Keycloak UserId in DMS.Authentication.
public class ImportedUser
{
    public int Id { get; set; }
    public required string ExternalId { get; set; }
    public required Guid KeycloakUserId { get; set; }
    public required string PartnerCode { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}
