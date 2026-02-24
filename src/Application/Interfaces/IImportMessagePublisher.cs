namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

public interface IImportMessagePublisher
{
    Task PublishJobCreatedAsync(Guid jobId);
}
