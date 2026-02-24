using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Messaging;

public class KafkaImportPublisher : IImportMessagePublisher
{
    private readonly IMessagePublisher _kafkaPublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaImportPublisher> _logger;
    private readonly string _importTopic;

    public KafkaImportPublisher(
        IMessagePublisher kafkaPublisher,
        IConfiguration configuration,
        ILogger<KafkaImportPublisher> logger)
    {
        _kafkaPublisher = kafkaPublisher;
        _configuration = configuration;
        _logger = logger;

        _importTopic = _configuration["KAFKA_IMPORT_TOPIC"]
            ?? throw new ArgumentNullException(nameof(configuration), "KAFKA_IMPORT_TOPIC is not set.");
    }

    public async Task PublishJobCreatedAsync(Guid jobId)
    {
        var payload = new ImportJobCreatedEvent { JobId = jobId };

        var messageId = Guid.NewGuid().ToString("N");
        var envelope = new KafkaMessage<ImportJobCreatedEvent>
        {
            Id = messageId,
            Content = payload,
            Timestamp = DateTime.UtcNow
        };

        var headers = new[]
        {
            new KeyValuePair<string, string>("content-type", "application/json"),
            new KeyValuePair<string, string>("x-channel", "import"),
            new KeyValuePair<string, string>("x-event-type", "job-created")
        };

        try
        {
            await _kafkaPublisher.PublishJsonAsync(
                route: _importTopic,
                key: jobId.ToString(),
                payload: envelope,
                headers: headers
            );

            _logger.LogInformation("Import job {JobId} published to Kafka topic {Topic}", jobId, _importTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish import job {JobId} to Kafka", jobId);
            throw;
        }
    }
}
