using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Confluent.Kafka;

using ExternalIntegrations.OrganizationChart.Application;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Infrastructure;

/// Background service that consumes from the "import-jobs" Kafka topic.

/// On receiving a message:
/// 1. Deserializes the KafkaMessage envelope
/// 2. Extracts JobId from the Content
/// 3. Creates a DI scope and resolves IImportService
/// 4. Calls ProcessJobAsync(jobId)
/// 5. Commits the offset ONLY after successful processing
///    (if processing fails, the message is redelivered on restart)

public class KafkaImportConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaImportConsumer> _logger;

    public KafkaImportConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<KafkaImportConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {       
        throw new NotImplementedException();
    }
}
