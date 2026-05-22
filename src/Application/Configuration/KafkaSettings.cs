using Microsoft.Extensions.Configuration;

namespace IntegrationImport.Application.Configuration;

public class KafkaSettings
{
    // Bootstrap servers
    public string BootstrapServers { get; set; } = string.Empty;

    // Topic for Import Job channel
    public string Topic { get; set; } = string.Empty;

    // Base delay before reconnecting to a broker
    public int ReconnectBackoffMs { get; set; }

    // Maximum delay when exponential backoff applies
    public int ReconnectBackoffMaxMs { get; set; }

    // Time allowed to establish initial TCP connection
    public int SocketConnectionSetupTimeoutMs { get; set; }

    // How long to wait for socket operations before failing
    public int SocketTimeoutMs { get; set; }

    // How many times the .NET client retries failed sends
    public int MessageSendMaxRetries { get; set; }

    // Wait between retries to avoid hammering the broker
    public int RetryBackoffMs { get; set; }

    // Max time broker has to respond to produce request
    public int RequestTimeoutMs { get; set; }

    // Max time before message is considered failed (client side)
    public int MessageTimeoutMs { get; set; }

    public string GroupId { get; set; } = "";
    public string AutoOffsetReset { get; set; } = "Earliest";
    public int SessionTimeoutMs { get; set; }
    public int MaxPollIntervalMs { get; set; }

    public static KafkaSettings BindFromConfiguration(IConfiguration configuration)
    {
        return new KafkaSettings
        {
            // Broker connection settings
            BootstrapServers = configuration["KAFKA_BOOTSTRAP_SERVERS"]
                ?? throw new ArgumentNullException(nameof(configuration), "KAFKA_BOOTSTRAP_SERVERS is not set."),
            ReconnectBackoffMs = int.Parse(
                configuration["IMPORT_KAFKA_RECONNECT_BACKOFF_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_RECONNECT_BACKOFF_MS is not set.")),
            ReconnectBackoffMaxMs = int.Parse(
                configuration["IMPORT_KAFKA_RECONNECT_BACKOFF_MAX_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_RECONNECT_BACKOFF_MAX_MS is not set.")),
            SocketConnectionSetupTimeoutMs = int.Parse(
                configuration["IMPORT_KAFKA_SOCKET_CONNECTION_SETUP_TIMEOUT_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_SOCKET_CONNECTION_SETUP_TIMEOUT_MS is not set.")),
            SocketTimeoutMs = int.Parse(
                configuration["IMPORT_KAFKA_SOCKET_TIMEOUT_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_SOCKET_TIMEOUT_MS is not set.")),

            // Producer settings
            Topic = configuration["IMPORT_KAFKA_TOPIC"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_TOPIC is not set."),

            RetryBackoffMs = int.Parse(
                configuration["IMPORT_KAFKA_RETRY_BACKOFF_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_RETRY_BACKOFF_MS is not set.")),
            RequestTimeoutMs = int.Parse(
                configuration["IMPORT_KAFKA_REQUEST_TIMEOUT_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_REQUEST_TIMEOUT_MS is not set.")),
            MessageTimeoutMs = int.Parse(
                configuration["IMPORT_KAFKA_MESSAGE_TIMEOUT_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_MESSAGE_TIMEOUT_MS is not set.")),

            // Consumer settings
            GroupId = configuration["IMPORT_KAFKA_CONSUMER_GROUP"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_CONSUMER_GROUP is not set."),
            AutoOffsetReset = configuration["IMPORT_KAFKA_AUTO_OFFSET_RESET"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_AUTO_OFFSET_RESET is not set."),
            SessionTimeoutMs = int.Parse(configuration["IMPORT_KAFKA_SESSION_TIMEOUT_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_SESSION_TIMEOUT_MS is not set.")),
            MaxPollIntervalMs = int.Parse(configuration["IMPORT_KAFKA_MAX_POLL_INTERVAL_MS"]
                ?? throw new ArgumentNullException(nameof(configuration), "IMPORT_KAFKA_MAX_POLL_INTERVAL_MS is not set.")),
        };
    }
}
