namespace IntegrationImport.Domain.Entities;

/// Mapping table: links the partner's ExternalId to the Keycloak UserId in DMS.Authentication.
public class ImportedUser
{
    public Guid Id { get; set; }

    // NEW — idempotency anchor (the UserImportItem.Id that produced this row)
    public Guid? SourceImportItemId { get; set; }

    public required string ExternalId { get; set; }

    // CHANGED — was: required Guid. Now nullable, because the reservation row
    // is created BEFORE we know the Keycloak id.
    public Guid? KeycloakUserId { get; set; }  

    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
    public Guid? LastImportRequestId { get; set; }
    public Guid? LastImportItemId { get; set; }
    public string? LastAction { get; set; }
    public string? LastSyncStatus { get; set; }

    public ICollection<ImportedUserOrganizationUnit> OrganizationUnits { get; set; } = new List<ImportedUserOrganizationUnit>();
}