namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class ImportJobReportDto
{
    public ImportJobResultDto Job { get; set; } = null!;
    public List<ImportJobItemResultDto> Items { get; set; } = new();
}
