namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class ImportBatchRequest
{
    public required string PartnerCode { get; set; }
    public required List<UserExternalDto> Users { get; set; }
    public bool IsReconciliation { get; set; } = false;
}
