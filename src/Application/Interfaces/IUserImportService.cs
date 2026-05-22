using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IUserImportService
{
    /// <summary>
    /// Ingests a user-import batch under partial-success semantics:
    /// validated items are persisted and queued for the review/dispatch pipeline,
    /// while rejected items are returned per-row in the response. The HRMS caller
    /// is expected to resend rejected items in a new batch with a fresh ReferenceId.
    /// </summary>
    Task<Result<PartialImportResponseDto>> ImportUsersAsync(UsersImportRequestDto request, CancellationToken ct);
}
