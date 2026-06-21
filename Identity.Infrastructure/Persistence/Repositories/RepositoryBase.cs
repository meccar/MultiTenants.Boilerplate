using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class
{
    private readonly AppDbContext _context;

    public RepositoryBase(AppDbContext context)
        => _context = context ?? throw new ArgumentNullException(nameof(context));

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private Expression<Func<TEntity, bool>> BuildKeyPredicate(TKey id)
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(
                $"Entity '{typeof(TEntity).Name}' is not part of the DbContext model.");

        var keyProperty = entityType.FindPrimaryKey()?.Properties.SingleOrDefault()
            ?? throw new InvalidOperationException(
                $"Entity '{typeof(TEntity).Name}' must have a single primary key.");

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property  = Expression.Call(
            typeof(EF), nameof(EF.Property), [typeof(TKey)],
            parameter, Expression.Constant(keyProperty.Name));
        var body = Expression.Equal(property, Expression.Constant(id, typeof(TKey)));

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private async Task<TEntity> GetEntityOrThrowAsync(TKey id, CancellationToken ct)
        => await _context.Set<TEntity>().FirstOrDefaultAsync(BuildKeyPredicate(id), ct)
           ?? throw new KeyNotFoundException(
               $"{typeof(TEntity).Name} with id '{id}' was not found.");

    private static ISoftDeletable GetSoftDeletableOrThrow(TEntity entity)
        => entity as ISoftDeletable
           ?? throw new NotSupportedException(
               $"{typeof(TEntity).Name} does not support soft delete.");

    // -------------------------------------------------------------------------
    // Create
    // -------------------------------------------------------------------------

    public async Task<TEntity?> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Set<TEntity>().AddAsync(entity, ct);
        return entry.Entity;
    }

    public async Task<IEnumerable<TEntity>> CreateListAsync(
        IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var list = entities.ToList();
        foreach (var entity in list)
            await _context.Set<TEntity>().AddAsync(entity, ct);
        return list;
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    public Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _context.Set<TEntity>().Update(entity);
        return Task.FromResult<TEntity?>(entry.Entity);
    }

    public Task UpdateListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Set<TEntity>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    // -------------------------------------------------------------------------
    // Delete (hard)
    // -------------------------------------------------------------------------

    public async Task DeleteAsync(TKey id, CancellationToken ct = default)
        => _context.Set<TEntity>().Remove(await GetEntityOrThrowAsync(id, ct));

    public Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task DeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var entities = await _context.Set<TEntity>()
            .Where(e => idList.Contains(EF.Property<TKey>(e, "Id")))
            .ToListAsync(ct);
        _context.Set<TEntity>().RemoveRange(entities);
    }

    // -------------------------------------------------------------------------
    // Soft delete
    // -------------------------------------------------------------------------

    public Task SoftDeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        var softDeletable = GetSoftDeletableOrThrow(entity);
        softDeletable.IsDeleted = true;
        softDeletable.DeletedAt = DateTime.Now.Ticks;
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(TKey id, CancellationToken ct = default)
        => await SoftDeleteAsync(await GetEntityOrThrowAsync(id, ct), ct);

    public async Task SoftDeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
            await SoftDeleteAsync(entity, ct);
    }

    public async Task SoftDeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        foreach (var id in ids)
            await SoftDeleteAsync(id, ct);
    }

    // -------------------------------------------------------------------------
    // Restore
    // -------------------------------------------------------------------------

    public Task RestoreAsync(TEntity entity, CancellationToken ct = default)
    {
        var softDeletable = GetSoftDeletableOrThrow(entity);
        softDeletable.IsDeleted = false;
        softDeletable.DeletedAt = null;
        _context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task RestoreAsync(TKey id, CancellationToken ct = default)
        => await RestoreAsync(await GetEntityOrThrowAsync(id, ct), ct);

    // -------------------------------------------------------------------------
    // Query
    // -------------------------------------------------------------------------

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await _context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(BuildKeyPredicate(id), ct);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await _context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default) // ← was ExistAsync
        => await _context.Set<TEntity>()
            .AnyAsync(BuildKeyPredicate(id), ct);

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _context.Set<TEntity>().CountAsync(ct);

    public async Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken ct = default)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize   < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query      = _context.Set<TEntity>().AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var items      = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>
        {
            Items      = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize   = pageSize
        };
    }
}