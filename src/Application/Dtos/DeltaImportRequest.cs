namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class DeltaImportRequest
{
    public required string PartnerCode { get; set; }
    public required List<DeltaUserDto> Changes { get; set; }
}
