using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class ImportJobResultDto
{
    public Guid JobId { get; set; }
    public ImportJobType Type { get; set; }
    public ImportJobStatus Status { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkipCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
