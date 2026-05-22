namespace IntegrationImport.Domain.Entities;

public class ImportedOrganizationUnit
{
    public Guid Id { get; set; }
    public Guid? SourceImportItemId { get; set; }

    public required string ExternalId { get; set; }

    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public string? Email { get; set; }
    public string? Location { get; set; }
    public bool IsVirtual { get; set; }

    public string? ParentExternalId { get; set; }
    public Guid? ParentImportedOrganizationUnitId { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
    public Guid? LastImportRequestId { get; set; }
    public Guid? LastImportItemId { get; set; }
    public string? LastAction { get; set; }
    public string? LastSyncStatus { get; set; }

    public ICollection<ImportedUserOrganizationUnit> Users { get; set; } = new List<ImportedUserOrganizationUnit>();
}