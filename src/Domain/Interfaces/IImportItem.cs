using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Domain.Interfaces;

/// <summary>
/// Common shape for items that flow through the review/dispatch/process pipeline.
/// Implemented by UserImportItem and OrganizationUnitImportItem so the review and
/// status services can operate over either collection without branching on the
/// concrete type.
/// </summary>
public interface IImportItem
{
    Guid Id { get; }
    string HrmsId { get; }

    ImportItemReviewStatus ReviewStatus { get; set; }
    ImportItemProcessStatus? ProcessStatus { get; set; }

    string? ReviewedBy { get; set; }
    DateTime? ReviewedAtUtc { get; set; }
    string? ReviewNotes { get; set; }
}
