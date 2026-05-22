using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Domain.Interfaces;

public interface IImportRequestRepository
{
    Task<bool> ExistsAsync(string referenceId, ImportEntityType entityType, CancellationToken ct);
    Task AddAsync(ImportRequest syncRequest, CancellationToken ct);
    Task<ImportRequest?> GetBySyncIdAsync(Guid syncId, CancellationToken ct);      
    Task<ImportRequest?> GetTrackedByIdWithUserItemsAsync(Guid syncId, CancellationToken ct);
    Task<ImportRequest?> GetTrackedByIdWithItemsAsync(Guid syncId, CancellationToken ct);
    Task<ImportRequest?> GetTrackedByIdAsync(Guid syncId, CancellationToken ct);
   
}
