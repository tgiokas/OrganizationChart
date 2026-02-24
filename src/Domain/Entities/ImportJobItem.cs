using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Domain.Entities;

public class ImportJobItem
{
    public int Id { get; set; }
    public int ImportJobId { get; set; }
    public required string ExternalId { get; set; }
    public ImportAction Action { get; set; }
    public ImportJobItemStatus Status { get; set; } = ImportJobItemStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    // Snapshot of what was sent by the partner for this item
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public ImportJob ImportJob { get; set; } = null!;
}
