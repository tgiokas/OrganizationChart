namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishJsonAsync<T>(
        string route,
        string key,
        T payload,
        IEnumerable<KeyValuePair<string, string>>? headers = null,
        CancellationToken cancellationToken = default);
}
