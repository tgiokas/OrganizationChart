using ExternalIntegrations.OrganizationChart.Domain.Enums;

namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class DeltaUserDto : ExternalUserDto
{
    public required ImportAction Action { get; set; }
}
