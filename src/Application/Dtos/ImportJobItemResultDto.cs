using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class ImportJobItemResultDto
{
    public string ExternalId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public ImportAction Action { get; set; }
    public ImportJobItemStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
