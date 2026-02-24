using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Application.Services;

/// Handles the actual processing of import jobs.
/// Called by the KafkaImportConsumer.
public class ImportProcessingService : IImportProcessingService
{
    private readonly IImportJobRepository _jobRepository;
    private readonly IImportJobItemRepository _jobItemRepository;
    private readonly IImportedUserRepository _importedUserRepository;
    private readonly IAuthApiClient _authApiClient;

    public ImportProcessingService(
        IImportJobRepository jobRepository,
        IImportJobItemRepository jobItemRepository,
        IImportedUserRepository importedUserRepository,
        IAuthApiClient authApiClient)
    {
        _jobRepository = jobRepository;
        _jobItemRepository = jobItemRepository;
        _importedUserRepository = importedUserRepository;
        _authApiClient = authApiClient;
    }

    public Task ProcessJobAsync(Guid jobId)
    {
        //  Load ImportJob by jobId
        //  Set Status = Processing
        //  Load all ImportJobItems with Status = Pending (skip already processed on retry)
        //
        //  FOR EACH item (sequential):
        //       - Lookup ImportedUsers mapping by ExternalId
        //
        //       - If Action = Disable:
        //           - Call _authApiClient.DisableUserAsync(keycloakUserId)
        //           - Update item status
        //
        //       - If NOT found in mapping (new user):
        //           - Call _authApiClient.CreateUserAsync(...)
        //           - On success: save mapping to ImportedUsers table
        //           - Update item: Status = Success
        //           - On failure: Update item: Status = Failed + ErrorMessage
        //
        //       - If FOUND in mapping (existing user):
        //           - Compare fields (username, email, firstName, lastName, phone)
        //           - If changed: Call _authApiClient.UpdateUserAsync(...)
        //              - On success: update mapping record + item Status = Success
        //              - On failure: item Status = Failed + ErrorMessage
        //           - If unchanged: item Status = Skipped
        //
        //  After all items processed:
        //       - Count successes, failures, skips
        //       - Update ImportJob: SuccessCount, FailCount, SkipCount
        //       - Set Status = Completed (all ok) or CompletedWithErrors (some failed)
        //       - Set CompletedAt = DateTime.UtcNow
        //       - Save

        throw new NotImplementedException();
    }
}
