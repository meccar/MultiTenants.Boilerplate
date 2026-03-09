using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace MultiTenants.Boilerplate.Domain.Seedwork.Interface;
public interface IUnitOfWork : IDisposable
{
    DbConnection Connection { get; }
    DbTransaction? Transaction { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
