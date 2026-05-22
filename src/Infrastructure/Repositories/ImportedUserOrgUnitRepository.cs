using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace IntegrationImport.Infrastructure.Repositories;

public class ImportedUserOrganizationUnitRepository : IImportedUserOrganizationUnitRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ImportedUserOrganizationUnitRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ImportedUserOrganizationUnit>> GetByImportedUserIdAsync(
        Guid importedUserId, CancellationToken ct)
    {
        return await _dbContext.ImportedUserOrganizationUnits
            .Where(x => x.ImportedUserId == importedUserId)
            .ToListAsync(ct);
    }

    public async Task UpsertRelationsAsync(
        ImportedUser importedUser,
        IReadOnlyCollection<ImportedOrganizationUnit> orgUnits,
        Guid importRequestId,
        Guid importItemId,
        CancellationToken ct)
    {
        var existing = await _dbContext.ImportedUserOrganizationUnits
            .Where(x => x.ImportedUserId == importedUser.Id)
            .ToListAsync(ct);

        var desiredById = orgUnits.ToDictionary(o => o.Id);
        var existingById = existing.ToDictionary(x => x.ImportedOrganizationUnitId);
        var nowUtc = DateTime.UtcNow;

        // ---- ADD or REACTIVATE ----
        foreach (var ou in orgUnits)
        {
            if (existingById.TryGetValue(ou.Id, out var prior))
            {
                if (!prior.IsActive)
                {
                    prior.IsActive = true;
                    prior.LastSyncedAtUtc = nowUtc;
                    prior.LastImportRequestId = importRequestId;
                    prior.LastImportItemId = importItemId;
                    prior.ExternalSyncStatus = "PENDING";
                    prior.ExternalSyncError = null;
                }
                // else: already active nothing to do
            }
            else
            {
                await _dbContext.ImportedUserOrganizationUnits.AddAsync(new ImportedUserOrganizationUnit
                {
                    Id = Guid.NewGuid(),
                    ImportedUserId = importedUser.Id,
                    ImportedOrganizationUnitId = ou.Id,
                    UserExternalId = importedUser.ExternalId,
                    OrgUnitExternalId = ou.ExternalId,
                    IsActive = true,
                    CreatedAtUtc = nowUtc,
                    LastSyncedAtUtc = nowUtc,
                    LastImportRequestId = importRequestId,
                    LastImportItemId = importItemId,
                    ExternalSyncStatus = "PENDING",
                }, ct);
            }
        }

        // ---- REMOVE (soft) ----
        foreach (var rel in existing.Where(x => x.IsActive))
        {
            if (desiredById.ContainsKey(rel.ImportedOrganizationUnitId)) continue;

            rel.IsActive = false;
            rel.LastSyncedAtUtc = nowUtc;
            rel.LastImportRequestId = importRequestId;
            rel.LastImportItemId = importItemId;
            rel.ExternalSyncStatus = "PENDING";
            rel.ExternalSyncError = null;
        }
    }
}
