using ExternalIntegrations.OrganizationChart.Application.Dtos;

namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

/// Handles import submission and job querying.
/// Called by the ImportController (controller-facing).
public interface IImportService
{
    /// Accepts a batch import request, creates the job and items in DB,
    /// publishes a Kafka message, and returns the jobId.
    Task<Result<Guid>> SubmitBatchImportAsync(ImportBatchRequest request);

    /// Accepts a delta import request, creates the job and items in DB,
    /// publishes a Kafka message, and returns the jobId.
    Task<Result<Guid>> SubmitDeltaImportAsync(ImportDeltaRequest request);

    /// Returns the current status of an import job.
    Task<Result<ImportJobResultDto>> GetJobStatusAsync(Guid jobId);

    /// Returns the full report for an import job including per-user results.
    Task<Result<ImportJobReportDto>> GetJobReportAsync(Guid jobId);
}
