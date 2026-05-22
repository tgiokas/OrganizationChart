using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

using Confluent.Kafka;

using IntegrationImport.Application.Configuration;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Dtos.Messaging;
using IntegrationImport.Application.Interfaces;

namespace IntegrationImport.Infrastructure;

/// Background service that consumes from the "import-jobs" Kafka topic.

public class KafkaImportConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaImportConsumer> _logger;
    private readonly IConsumer<string, string> _consumer;

    private readonly string _importJobChannel;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public KafkaImportConsumer(
        IOptions<KafkaSettings> kafkaOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<KafkaImportConsumer> logger
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _importJobChannel = kafkaOptions.Value.Topic;

        var settings = kafkaOptions.Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            ReconnectBackoffMs = settings.ReconnectBackoffMs,
            ReconnectBackoffMaxMs = settings.ReconnectBackoffMaxMs,
            SocketConnectionSetupTimeoutMs = settings.SocketConnectionSetupTimeoutMs,
            SocketTimeoutMs = settings.SocketTimeoutMs,

            GroupId = settings.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(settings.AutoOffsetReset),
            EnableAutoCommit = false,
            SessionTimeoutMs = settings.SessionTimeoutMs,
            MaxPollIntervalMs = settings.MaxPollIntervalMs
        };


        _consumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetErrorHandler((_, e) =>
            {
                if (e.IsFatal)
                    _logger.LogError("Fatal Kafka error: {Reason}", e.Reason);
                else
                    _logger.LogWarning("Kafka error: {Reason}", e.Reason);
            })
            .SetLogHandler((_, msg) =>
            {
                _logger.LogDebug("Kafka[{Name}] {Level}: {Message}", msg.Name, msg.Level, msg.Message);
            })
            .Build();
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KafkaImportConsumer starting. Topic: {Topic}", _importJobChannel);

        try
        {
            _consumer.Subscribe(_importJobChannel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to topic {Topic}.", _importJobChannel);
            return;
        }

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;

                try
                {
                    result = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (result == null)
                    {
                        await Task.Delay(200, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation("Message from topic {Topic} consumed", result.Topic);

                    var itemDto = ParsePayload(result.Message.Value);
                    if (itemDto == null)
                    {
                        _logger.LogWarning("Skipping message at {TPO}: unable to parse payload.", result.TopicPartitionOffset);

                        // Commit to avoid  loops
                        _consumer.Commit(result);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var process = scope.ServiceProvider.GetRequiredService<IImportProcessingService>();

                    await process.ProcessApprovedItemAsync(itemDto, stoppingToken);

                    _consumer.Commit(result); // success

                    _logger.LogInformation("Offset commited at {TPO} ", result.TopicPartitionOffset);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "ConsumeException at {TPO}: {Reason}",
                        result?.TopicPartitionOffset, ex.Error.Reason);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "JSON error at {TPO}; committing to skip poison message.",
                        result?.TopicPartitionOffset);
                    if (result is not null) _consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled processing error at {TPO}. Backing off briefly.",
                        result?.TopicPartitionOffset);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        finally
        {
            try { _consumer.Close(); } // leave group & commit last offsets
            catch (Exception ex) { _logger.LogWarning(ex, "Error closing Kafka consumer."); }
        }
    }



    private static ImportΙtemApprovedMessageDto? ParsePayload(string payload)
    {
        // 1) Envelope with typed Content
        try
        {
            var env = JsonSerializer.Deserialize<KafkaMessage<ImportΙtemApprovedMessageDto>>(payload, JsonOpts);
            if (env?.Content is not null) return env.Content;
        }
        catch { /* fall through */ }

        // 2) Envelope with string Content
        try
        {
            var envRaw = JsonSerializer.Deserialize<KafkaMessage<string>>(payload, JsonOpts);
            if (!string.IsNullOrWhiteSpace(envRaw?.Content))
                return JsonSerializer.Deserialize<ImportΙtemApprovedMessageDto>(envRaw.Content, JsonOpts);
        }
        catch { /* fall through */ }

        // 3) Bare DTO
        try
        {
            return JsonSerializer.Deserialize<ImportΙtemApprovedMessageDto>(payload, JsonOpts);
        }
        catch { }

        return null;

    }

}
