using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Confluent.Kafka;

using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Messaging;

/// Background service that consumes from the "import-jobs" Kafka topic.
/// 
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
        // 1. Build ConsumerConfig:
        //    var consumerConfig = new ConsumerConfig
        //    {
        //        BootstrapServers = _configuration["KAFKA_BOOTSTRAP_SERVERS"],
        //        GroupId = _configuration["KAFKA_CONSUMER_GROUP"],
        //        AutoOffsetReset = AutoOffsetReset.Earliest,
        //        EnableAutoCommit = false  // manual commit after processing
        //    };
        //
        // 2. Build consumer and subscribe:
        //    using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        //    consumer.Subscribe(_configuration["KAFKA_IMPORT_TOPIC"]);
        //
        // 3. Consume loop:
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var consumeResult = consumer.Consume(stoppingToken);
        //
        //        // 4. Deserialize the KafkaMessage<ImportJobCreatedEvent> envelope:
        //        var envelope = JsonSerializer.Deserialize<KafkaMessage<ImportJobCreatedEvent>>(consumeResult.Message.Value);
        //        var jobId = envelope?.Content?.JobId;
        //
        //        // 5. Create DI scope (because this is a singleton hosted service):
        //        using var scope = _scopeFactory.CreateScope();
        //        var importService = scope.ServiceProvider.GetRequiredService<IImportService>();
        //
        //        // 6. Process the job:
        //        await importService.ProcessJobAsync(jobId.Value);
        //
        //        // 7. Commit offset ONLY after successful processing:
        //        consumer.Commit(consumeResult);
        //
        //        // If ProcessJobAsync throws, offset is NOT committed.
        //        // On restart, the consumer will re-read this message.
        //        // ProcessJobAsync skips items already marked Success (idempotent).
        //    }

        throw new NotImplementedException();
    }
}
