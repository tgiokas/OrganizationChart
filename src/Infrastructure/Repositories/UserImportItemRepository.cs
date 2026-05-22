using Microsoft.EntityFrameworkCore;

using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.Database;
using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Repositories;

public class UserImportItemRepository: IUserImportItemRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserImportItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<UserImportItem>> GetApprovedPendingDispatchUserItemsAsync(int batchSize, CancellationToken ct)
    {
        return _dbContext.UserImportItems
            .Include(x => x.ImportRequest)
            .Where(x =>
                x.ReviewStatus == ImportItemReviewStatus.APPROVED &&
                x.ProcessStatus == ImportItemProcessStatus.PENDING_DISPATCH &&
                x.DispatchedAtUtc == null)
            .OrderBy(x => x.ReviewedAtUtc)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public Task<UserImportItem?> GetTrackedUserItemByIdAsync(Guid itemId, CancellationToken ct)
    {
        return _dbContext.UserImportItems
            .Include(x => x.ImportRequest)
            .FirstOrDefaultAsync(x => x.Id == itemId, ct);
    }
}
