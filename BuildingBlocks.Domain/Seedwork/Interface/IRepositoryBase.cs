using BuildingBlocks.Domain.Seedwork.Aggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Seedwork.Interface;
public interface IRepositoryBase<T> where T : AggregateRoot
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

    Task<EntityEntry<T>?> CreateAsync(T entity);
    Task<IEnumerable<EntityEntry<T>>> CreateListAsync(IEnumerable<T> entities);

    #endregion

    #region Update Methods

    Task<EntityEntry<T>?> UpdateAsync(T entity);
    Task UpdateListAsync(IEnumerable<T> entities);

    #endregion

    #region Deletion and Restoration Methods

    Task DeleteAsync(string id);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(T entity);
    Task DeleteListAsync(IEnumerable<T> entities);
    Task DeleteListAsync(IEnumerable<string> ids);
    Task DeleteListAsync(IEnumerable<Guid> ids);
    Task SoftDeleteAsync(T entity);
    Task SoftDeleteAsync(string id);
    Task SoftDeleteAsync(Guid id);
    Task SoftDeleteListAsync(IEnumerable<T> entities);
    Task SoftDeleteListAsync(IEnumerable<string> ids);
    Task SoftDeleteListAsync(IEnumerable<Guid> ids);
    Task RestoreAsync(T entity);
    Task RestoreAsync(string id);
    Task RestoreAsync(Guid id);

    #endregion

    #region Query Methods

    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);
    Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false);
    Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false);
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);
    Task<bool> ExistAsync(string id);
    Task<bool> ExistAsync(Guid id);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> expression);
    Task<T?> GetByIdAsync(string id);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

    #endregion
}

