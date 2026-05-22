using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Domain.Interfaces
{
    public interface IImportedUserOrganizationUnitRepository
    {
        Task<List<ImportedUserOrganizationUnit>> GetByImportedUserIdAsync(Guid importedUserId, CancellationToken ct);



        //1. Get all relations for a list of imported user ids
        //2. Όσα org units υπάρχουν στο νέο payload → active
        //3. Όσα org units δεν υπάρχουν στο νέο payload → inactive
        Task UpsertRelationsAsync(
            ImportedUser importedUser,
            IReadOnlyCollection<ImportedOrganizationUnit> orgUnits,
            Guid importRequestId,
            Guid importItemId,
            CancellationToken ct);
    }
}
