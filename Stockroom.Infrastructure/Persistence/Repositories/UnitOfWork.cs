using BuildingBlocks.Core.Seedwork.Interface;
using Stockroom.Infrastructure.Persistence.Data;

namespace Stockroom.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _disposed;

    public UnitOfWork(
        AppDbContext context
    )
    {
        _context = context;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
                => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
            => await _context.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _context.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTrancsactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTrancsactionAsync();
        }
    }
    
    private async Task DisposeTrancsactionAsync()
    {
        var currentTransaction = _context.CurrentTransaction;
        if (currentTransaction != null)
            await currentTransaction.DisposeAsync();
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            _context.Dispose();
        _disposed = true;
    }
        
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;
        await _context.DisposeAsync();
        _disposed = true;
    }
}