using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace IntegrationImport.Infrastructure.Repositories;

public class OrgUnitImportItemRepository : IOrganizationUnitImportItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrgUnitImportItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<OrganizationUnitImportItem>> GetApprovedPendingDispatchOrgUnitItemsAsync(
        int batchSize, CancellationToken ct)
    {
        return _dbContext.OrganizationUnitImportItems
            .Include(x => x.ImportRequest)
            .Where(x =>
                x.ReviewStatus == ImportItemReviewStatus.APPROVED &&
                x.ProcessStatus == ImportItemProcessStatus.PENDING_DISPATCH &&
                x.DispatchedAtUtc == null)
            .OrderBy(x => x.ReviewedAtUtc)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public Task<OrganizationUnitImportItem?> GetTrackedOrgUnitItemByIdAsync(
        Guid itemId, CancellationToken ct)
    {
        return _dbContext.OrganizationUnitImportItems
            .Include(x => x.ImportRequest)
            .FirstOrDefaultAsync(x => x.Id == itemId, ct);
    }
}
