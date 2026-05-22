using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IReviewStatusService
{
    Task<Result<ImportStatusResponseDto>?> GetStatusAsync(Guid syncId, CancellationToken ct);
    Task<Result<ImportStatusResponseDto>> ReviewItemsAsync(ReviewImportRequestDto request, CancellationToken ct);
}
