using MultiTenants.Boilerplate.Domain.Seedwork.Interface;
using MultiTenants.Boilerplate.Infrastructure.Persistance.Data;

namespace MultiTenants.Boilerplate.Infrastructure.Persistance.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _disposed;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch
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
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }
        finally
        {
            await DisposeTrancsactionAsync();
        }
    }

    private async Task DisposeTrancsactionAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
            await currentTransaction.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if(disposing)
            _context.Dispose();
        _disposed = true;
    }
}
