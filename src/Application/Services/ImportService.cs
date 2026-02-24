using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Application.Services;

/// Handles import submission and job querying.
/// Called by the ImportController.
public class ImportService : IImportService
{
    private readonly IImportJobRepository _jobRepository;
    private readonly IImportJobItemRepository _jobItemRepository;
    private readonly IImportedUserRepository _importedUserRepository;
    private readonly IImportMessagePublisher _messagePublisher;

    public ImportService(
        IImportJobRepository jobRepository,
        IImportJobItemRepository jobItemRepository,
        IImportedUserRepository importedUserRepository,     
        IImportMessagePublisher messagePublisher)
    {
        _jobRepository = jobRepository;
        _jobItemRepository = jobItemRepository;
        _importedUserRepository = importedUserRepository;      
        _messagePublisher = messagePublisher;
    }

    public Task<Result<Guid>> SubmitBatchImportAsync(BatchImportRequest request)
    {
        //  Validate request
        //  Create ImportJob entity (Status = Pending, Type = Batch)
        //  Create ImportJobItem entities for each user
        //  Save to DB
        //  Publish Kafka message with jobId
        //  Return jobId
        throw new NotImplementedException();
    }

    public Task<Result<Guid>> SubmitDeltaImportAsync(DeltaImportRequest request)
    {
        //  Validate request
        //  Create ImportJob entity (Status = Pending, Type = Delta)
        //  Create ImportJobItem entities for each change (with explicit Action)
        //  Save to DB
        //  Publish Kafka message with jobId
        //  Return jobId
        throw new NotImplementedException();
    }

    public Task<Result<ImportJobResultDto>> GetJobStatusAsync(Guid jobId)
    {
        //  Load ImportJob by jobId
        //  Map to ImportJobResultDto
        //  Return
        throw new NotImplementedException();
    }

    public Task<Result<ImportJobReportDto>> GetJobReportAsync(Guid jobId)
    {
        //  Load ImportJob with Items by jobId
        //  Map to ImportJobReportDto with list of ImportJobItemResultDto
        //  Return
        throw new NotImplementedException();
    }
}
