using System.Data;
using System.Data.Common;
using BuildingBlocks.Core.Seedwork.DomainEvent;
using BuildingBlocks.Core.Seedwork.Interface;
using IAsyncDisposable = System.IAsyncDisposable;
using IDisposable = System.IDisposable;

namespace BuildingBlocks.Core.Seedwork.Context;

public class TransactionContext 
    : IDisposable, IAsyncDisposable
{
    private static readonly ILogger logger = LoggerBase.Get(typeof(TransactionContext));
    internal DbConnection Connection { get; }
    internal DbTransaction Transaction { get; private set; }

    internal TransactionContext(DbConnection connection)
    {
        Connection = connection;
    }

    internal void BeginTransaction()
    {
        Transaction = Connection.BeginTransaction();
    }

    internal async Task BeginTransactionAsync()
    {
        Transaction = await Connection.BeginTransactionAsync();
    }

    internal TransactionContext(DbConnection connection, IsolationLevel isolationLevel)
    {
        Connection = connection;
    }

    public void Commit()
    {
        Transaction.Commit();
    }

    public async Task CommitAsync()
    {
        await Transaction.CommitAsync();
    }

    public void Rollback()
    {
        try
        {
            Transaction.Rollback();
        }
        catch (Exception e)
        {
            logger.Error($"Failed to rollback transaction, Error: {e}", nameof(Rollback), nameof(TransactionContext), 90);
        }
    }
    
    public async Task RollbackAsync()
    {
        try
        {
            await Transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            logger.Error($"Failed to rollback transaction, Error: {e}", nameof(RollbackAsync), nameof(TransactionContext), 102);
        }
    }

    public void Dispose()
    {
        try
        {
            Transaction.Dispose();
            Connection.Dispose();
            GC.SuppressFinalize(this);
        }
        catch (Exception e)
        {
            logger.Error($"Failed to dispose transaction, Error: {e}", nameof(Dispose), nameof(TransactionContext), 116);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _ = 1;
        try
        {
            await Transaction.DisposeAsync();
            await Connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
        catch (Exception e)
        {
            logger.Error($"Failed to dispose transaction, Error: {e}", nameof(DisposeAsync), nameof(TransactionContext), 130);
        }
    }
}