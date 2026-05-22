using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Domain.Entities;

public class UserImportItem : IImportItem
{
    public Guid Id { get; set; }
    public Guid ImportRequestId { get; set; }
    public string HrmsId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? Enabled { get; set; }
    public string? VatId { get; set; }

    /// <summary>
    /// JSON array string of changed fields from HRMS.
    /// Example: ["email","phoneNumber"]
    /// </summary>
    public string? ChangedFieldsJson { get; set; }

    /// <summary>
    /// JSON array string of org unit ids from HRMS.
    /// Example: ["OU-100","OU-120"]
    /// </summary>
    public string? OrgUnitHrmsIdsJson { get; set; }

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
