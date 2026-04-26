using System.Linq.Expressions;
using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Identity.Infrastructure.Persistance.Repositories;

public class RepositoryBase<TEntity, TKey> 
    : IRepositoryBase<TEntity, TKey> where TEntity : class
{
    protected readonly AppDbContext _context;

    public RepositoryBase(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    private IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includeProperties)
        => includeProperties.Aggregate(query, (q, include) => q.Include(include));

    private Expression<Func<TEntity, bool>> BuildKeyPredicate(TKey id)
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"Entity '{typeof(TEntity).Name}' is not part of the DbContext model.");
        var keyProperty = entityType.FindPrimaryKey()?.Properties.SingleOrDefault()
            ?? throw new InvalidOperationException($"Entity '{typeof(TEntity).Name}' must have a single primary key.");

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(TKey)],
            parameter,
            Expression.Constant(keyProperty.Name));
        var body = Expression.Equal(property, Expression.Constant(id, typeof(TKey)));

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private async Task<TEntity> GetEntityOrThrowAsync(TKey id)
        => await _context.Set<TEntity>().FirstOrDefaultAsync(BuildKeyPredicate(id))
           ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with id '{id}' was not found.");

    private static ISoftDeletable GetSoftDeletableOrThrow(TEntity entity)
        => entity as ISoftDeletable
           ?? throw new NotSupportedException($"{typeof(TEntity).Name} does not support soft delete.");
    
    public async Task<EntityEntry<TEntity>?> CreateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return await _context.Set<TEntity>().AddAsync(entity);
    }

    public async Task<IEnumerable<EntityEntry<TEntity>>> CreateListAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var entries = new List<EntityEntry<TEntity>>();
        foreach (var entity in entities)
            entries.Add(await _context.Set<TEntity>().AddAsync(entity));
        return entries;
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    public Task<EntityEntry<TEntity>?> UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return Task.FromResult<EntityEntry<TEntity>?>(_context.Set<TEntity>().Update(entity));
    }

    public Task UpdateListAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Set<TEntity>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    // ─── Deletion ─────────────────────────────────────────────────────────────

    public async Task DeleteAsync(TKey id)
        => _context.Set<TEntity>().Remove(await GetEntityOrThrowAsync(id));

    public Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteListAsync(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task DeleteListAsync(IEnumerable<TKey> ids)
    {
        var idList = ids.ToList();
        var entities = await _context.Set<TEntity>()
            .Where(entity => idList.Contains(EF.Property<TKey>(entity, "Id")))
            .ToListAsync();
        _context.Set<TEntity>().RemoveRange(entities);
    }

    // ─── Soft Delete ──────────────────────────────────────────────────────────

    public Task SoftDeleteAsync(TEntity entity)
    {
        var softDeletable = GetSoftDeletableOrThrow(entity);
        softDeletable.IsDeleted = true;
        softDeletable.DeletedAt = DateTime.UtcNow;
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(TKey id)
        => await SoftDeleteAsync(await GetEntityOrThrowAsync(id));

    public async Task SoftDeleteListAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities) await SoftDeleteAsync(entity);
    }

    public async Task SoftDeleteListAsync(IEnumerable<TKey> ids)
    {
        foreach (var id in ids) await SoftDeleteAsync(id);
    }

    // ─── Restore ──────────────────────────────────────────────────────────────

    public Task RestoreAsync(TEntity entity)
    {
        var softDeletable = GetSoftDeletableOrThrow(entity);
        softDeletable.IsDeleted = false;
        softDeletable.DeletedAt = null;
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task RestoreAsync(TKey id)
        => await RestoreAsync(await GetEntityOrThrowAsync(id));

    // ─── Query ────────────────────────────────────────────────────────────────

    public IQueryable<TEntity> FindAll(bool trackChanges = false)
        => trackChanges ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();

    public IQueryable<TEntity> FindAll(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
        => ApplyIncludes(FindAll(trackChanges), includeProperties);

    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false)
        => FindAll(trackChanges).Where(expression);

    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties)
        => ApplyIncludes(FindByCondition(expression, trackChanges), includeProperties);

    public async Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false)
        => await FindAll(trackChanges).ToListAsync();

    public async Task<IEnumerable<TEntity>> FindAllAsync(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(trackChanges, includeProperties).ToListAsync();

    public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false)
        => await FindByCondition(expression, trackChanges).ToListAsync();

    public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindByCondition(expression, trackChanges, includeProperties).ToListAsync();
    
    public async Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
        => await GetPagedAsync(pageNumber, pageSize, null, null, true, false);

    public async Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? expression = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = FindAll(trackChanges, includeProperties);

        if (expression is not null) query = query.Where(expression);

        var totalCount = await query.CountAsync();

        if (orderBy is not null)
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // ─── Exist / Count ────────────────────────────────────────────────────────

    public async Task<bool> ExistAsync(TKey id)
        => await _context.Set<TEntity>().AnyAsync(BuildKeyPredicate(id));

    public async Task<int> CountAsync()
        => await _context.Set<TEntity>().CountAsync();

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        => await _context.Set<TEntity>().CountAsync(expression);

    // ─── GetById ──────────────────────────────────────────────────────────────

    public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _context.Set<TEntity>().FirstOrDefaultAsync(BuildKeyPredicate(id));

    public async Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includeProperties)
        => await ApplyIncludes(_context.Set<TEntity>().AsNoTracking(), includeProperties)
            .FirstOrDefaultAsync(BuildKeyPredicate(id));
}
