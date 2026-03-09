namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class ImportDeltaRequest
{
    public required string PartnerCode { get; set; }
    public required List<UserDeltaDto> Changes { get; set; }
}
