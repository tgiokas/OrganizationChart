using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Application.Services;

public class ImportService : IImportService
{
    private readonly IImportJobRepository _jobRepository;
    private readonly IImportJobItemRepository _jobItemRepository;
    private readonly IImportedUserRepository _importedUserRepository;
    private readonly IAuthApiClient _authApiClient;
    private readonly IImportMessagePublisher _messagePublisher;

    public ImportService(
        IImportJobRepository jobRepository,
        IImportJobItemRepository jobItemRepository,
        IImportedUserRepository importedUserRepository,
        IAuthApiClient authApiClient,
        IImportMessagePublisher messagePublisher)
    {
        _jobRepository = jobRepository;
        _jobItemRepository = jobItemRepository;
        _importedUserRepository = importedUserRepository;
        _authApiClient = authApiClient;
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

    public Task ProcessJobAsync(Guid jobId)
    {
        //  Load ImportJob by jobId
        //  Set Status = Processing
        //  Load all ImportJobItems with Status = Pending
        //  FOR EACH item (sequential):
        //       - Lookup ImportedUsers mapping by ExternalId
        //       - If NOT found --> call _authApiClient.CreateUserAsync --> save mapping
        //       - If FOUND + changed --> call _authApiClient.UpdateUserAsync
        //       - If FOUND + unchanged --> mark as Skipped
        //       - If Action = Disable --> call _authApiClient.DisableUserAsync
        //       - Update item status (Success/Failed/Skipped)
        //  Update ImportJob totals and final status
        //  Set CompletedAt
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
