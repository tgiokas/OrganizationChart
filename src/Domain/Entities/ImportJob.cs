using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Domain.Entities;

public class ImportJob
{
    public int Id { get; set; }
    public required Guid JobId { get; set; }
    public ImportJobType Type { get; set; }
    public ImportJobStatus Status { get; set; } = ImportJobStatus.Pending;
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkipCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public List<ImportJobItem> Items { get; set; } = new();
}
