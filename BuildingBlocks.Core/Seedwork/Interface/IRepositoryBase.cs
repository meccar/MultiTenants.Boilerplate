using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace BuildingBlocks.Core.Seedwork.Interface;
public interface IRepositoryBase<TEntity, TKey>
    where TEntity : class
{
    public class PagedResult<TItem>
    {
        public IEnumerable<TItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    #region Creation Methods

    Task<EntityEntry<TEntity>?> CreateAsync(TEntity entity);
    Task<IEnumerable<EntityEntry<TEntity>>> CreateListAsync(IEnumerable<TEntity> entities);

    #endregion

    #region Update Methods

    Task<EntityEntry<TEntity>?> UpdateAsync(TEntity entity);
    Task UpdateListAsync(IEnumerable<TEntity> entities);

    #endregion

    #region Deletion and Restoration Methods

    Task DeleteAsync(TKey id);
    Task DeleteAsync(TEntity entity);
    Task DeleteListAsync(IEnumerable<TEntity> entities);
    Task DeleteListAsync(IEnumerable<TKey> ids);
    Task SoftDeleteAsync(TEntity entity);
    Task SoftDeleteAsync(TKey id);
    Task SoftDeleteListAsync(IEnumerable<TEntity> entities);
    Task SoftDeleteListAsync(IEnumerable<TKey> ids);
    Task RestoreAsync(TEntity entity);
    Task RestoreAsync(TKey id);

    #endregion

    #region Query Methods

    Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? expression = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties);
    IQueryable<TEntity> FindAll(bool trackChanges = false);
    IQueryable<TEntity> FindAll(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties);
    IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false);
    IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false);
    Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false);
    Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties);
    Task<bool> ExistAsync(TKey id);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includeProperties);

    #endregion
}
