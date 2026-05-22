using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Domain.Entities;

public class ImportRequest
{
    public Guid Id { get; set; }
    public string ReferenceId { get; set; } = null!;
    public DateOnly EffectiveDate { get; set; }
    public ImportJobStatus Status { get; set; }
    public ImportEntityType EntityType { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int ItemCount { get; set; }
    public string? Notes { get; set; }
    public string? CreatedByClientId { get; set; }
    public ICollection<UserImportItem> Users { get; set; } = new List<UserImportItem>();
    public ICollection<OrganizationUnitImportItem> OrganizationUnits { get; set; } = new List<OrganizationUnitImportItem>();

}
