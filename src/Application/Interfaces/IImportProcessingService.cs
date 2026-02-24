namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

/// Handles the actual processing of import jobs.
/// Called by the KafkaImportConsumer (consumer-facing).
public interface IImportProcessingService
{
    /// Processes all pending items for the given job sequentially.
    /// For each item: lookup mapping --> call DMS.Auth --> update status.
    /// Skips items already marked as Success (idempotent on retry).
    Task ProcessJobAsync(Guid jobId);
}
