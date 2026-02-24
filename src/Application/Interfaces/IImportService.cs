using ExternalIntegrations.OrganizationChart.Application.Dtos;

namespace ExternalIntegrations.OrganizationChart.Application.Interfaces;

public interface IImportService
{
    Task<Result<Guid>> SubmitBatchImportAsync(BatchImportRequest request);

    Task<Result<Guid>> SubmitDeltaImportAsync(DeltaImportRequest request);

    Task ProcessJobAsync(Guid jobId);

    Task<Result<ImportJobResultDto>> GetJobStatusAsync(Guid jobId);

    Task<Result<ImportJobReportDto>> GetJobReportAsync(Guid jobId);
}
