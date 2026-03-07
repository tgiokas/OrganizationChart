namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class BatchImportRequest
{
    public required string PartnerCode { get; set; }
    public required List<ExternalUserDto> Users { get; set; }
    public bool IsReconciliation { get; set; } = false;
}
