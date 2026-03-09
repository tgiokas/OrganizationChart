using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class UserDeltaDto : ExternalUserDto
{
    public required ImportAction Action { get; set; }
}
