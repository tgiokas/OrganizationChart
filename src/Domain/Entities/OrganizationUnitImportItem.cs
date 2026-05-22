using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Domain.Entities;

public class OrganizationUnitImportItem : IImportItem
{
    public Guid Id { get; set; }
    public Guid ImportRequestId { get; set; }
    public string HrmsId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public string? ParentHrmsId { get; set; }
    public string? Email { get; set; }
    public string? Location { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVirtual { get; set; }

    public string? ChangedFieldsJson { get; set; }

    public ImportItemReviewStatus ReviewStatus { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewNotes { get; set; }

    public ImportItemProcessStatus? ProcessStatus { get; set; }
    public DateTime? DispatchedAtUtc { get; set; }
    public int DispatchAttempts { get; set; }
    public string? LastDispatchError { get; set; }
    public DateTime? AppliedAtUtc { get; set; }
    public string? ApplyError { get; set; }

    public ImportRequest ImportRequest { get; set; } = null!;
}
