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

    Task<TEntity?> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> CreateListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    #endregion

    #region Update Methods

    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    #endregion

    #region Deletion and Restoration Methods

    Task DeleteAsync(TKey id, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    Task DeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default);
    Task SoftDeleteAsync(TKey id, CancellationToken ct = default);
    Task SoftDeleteAsync(TEntity entity, CancellationToken ct = default);
    Task SoftDeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default);
    Task SoftDeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    Task RestoreAsync(TKey id, CancellationToken ct = default);
    Task RestoreAsync(TEntity entity, CancellationToken ct = default);

    #endregion

    #region Query Methods

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    #endregion
}
