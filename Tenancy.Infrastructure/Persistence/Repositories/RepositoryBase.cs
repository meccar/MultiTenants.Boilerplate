using System.Linq.Expressions;
using BuildingBlocks.Core.Seedwork.Interface;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tenancy.Infrastructure.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey> where TEntity : class
{
    public Task<EntityEntry<TEntity>?> CreateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EntityEntry<TEntity>>> CreateListAsync(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task<EntityEntry<TEntity>?> UpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateListAsync(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteListAsync(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task DeleteListAsync(IEnumerable<TKey> ids)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteListAsync(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteListAsync(IEnumerable<TKey> ids)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? expression = null, Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true, bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> FindAll(bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> FindAll(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> GetByIdAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }
}