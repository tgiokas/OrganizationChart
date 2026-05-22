using Microsoft.EntityFrameworkCore.Storage;

using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Infrastructure.Database;

public class DbTransactionScope : IDbTransactionScope
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public DbTransactionScope(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _dbContext.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        if (_currentTransaction is not null)
            return;

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _dbContext.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        catch
        {
            await _currentTransaction.RollbackAsync(ct);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct)
    {
        await BeginTransactionAsync(ct);

        try
        {
            await action();
            await CommitTransactionAsync(ct);
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
    }
}