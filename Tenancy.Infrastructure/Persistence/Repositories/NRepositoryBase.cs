using BuildingBlocks.Core.Seedwork.DomainEvent;
using BuildingBlocks.Core.Seedwork.Entity;
using BuildingBlocks.Core.Seedwork.Interface;

namespace Tenancy.Infrastructure.Persistence.Repositories;

public abstract class NRepositoryBase<TDomain, TEntity>
    where TDomain : DomainBase
    where TEntity : EntityBase
{
    protected readonly string tableName = typeof(TEntity).Name;
    protected readonly IDatabaseHelper DbHelper;
}