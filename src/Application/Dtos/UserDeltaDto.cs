using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class UserDeltaDto : UserExternalDto
{
    public required ImportAction Action { get; set; }
}
