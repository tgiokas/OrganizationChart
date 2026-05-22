namespace IntegrationImport.Domain.Entities;

public class ImportedUserOrganizationUnit
{
    public Guid Id { get; set; }

    public Guid ImportedUserId { get; set; }
    public ImportedUser ImportedUser { get; set; } = null!;

    public Guid ImportedOrganizationUnitId { get; set; }
    public ImportedOrganizationUnit ImportedOrganizationUnit { get; set; } = null!;

    public string UserExternalId { get; set; } = null!;
    public string OrgUnitExternalId { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }

    public Guid? LastImportRequestId { get; set; }
    public Guid? LastImportItemId { get; set; }

    public string? ExternalSyncStatus { get; set; } // PENDING, SENT, FAILED
    public DateTime? ExternalSyncedAtUtc { get; set; }
    public string? ExternalSyncError { get; set; }
}