using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BuildingBlocks.Domain.Seedwork.Entity;
using BuildingBlocks.Domain.Seedwork.Interface;
using BuildingBlocks.Infrastructure.Persistance.Data;
using System.Linq.Expressions;

namespace BuildingBlocks.Infrastructure.Persistance.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
{
    protected readonly AppDbContext _context;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly DbSet<T> _dbSet;

    protected RepositoryBase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _dbSet = context.Set<T>();
    }

    // ── Queryable ────────────────────────────────────────────────────────────

    public IQueryable<T> FindAll(bool trackChanges = false)
        => trackChanges ? _dbSet : _dbSet.AsNoTracking();

    public IQueryable<T> FindAll(bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
        => includeProperties.Aggregate(FindAll(trackChanges),
            (query, prop) => query.Include(prop));

    public IQueryable<T> FindByCondition(
        Expression<Func<T, bool>> expression, bool trackChanges = false)
        => FindAll(trackChanges).Where(expression);

    public IQueryable<T> FindByCondition(
        Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
        => FindAll(trackChanges, includeProperties).Where(expression);

    public async Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false)
        => await FindAll(trackChanges).ToListAsync();

    public async Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
        => await FindAll(trackChanges, includeProperties).ToListAsync();

    public async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression, bool trackChanges = false)
        => await FindByCondition(expression, trackChanges).ToListAsync();

    public async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
        => await FindByCondition(expression, trackChanges, includeProperties).ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<T?> GetByIdAsync(string id)
        => await GetByIdAsync(Guid.Parse(id));

    public async Task<T?> GetByIdAsync(Guid id,
        params Expression<Func<T, object>>[] includeProperties)
        => await FindAll(false, includeProperties)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<T?> GetByIdAsync(string id,
        params Expression<Func<T, object>>[] includeProperties)
        => await GetByIdAsync(Guid.Parse(id), includeProperties);

    public async Task<bool> ExistAsync(Guid id)
        => await _dbSet.AnyAsync(e => e.Id == id);

    public async Task<bool> ExistAsync(string id)
        => await ExistAsync(Guid.Parse(id));

    public async Task<int> CountAsync()
        => await _dbSet.CountAsync();

    public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        => await _dbSet.CountAsync(expression);

    public async Task<EntityEntry<T>?> CreateAsync(T entity)
    {
        var entry = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<IEnumerable<EntityEntry<T>>> CreateListAsync(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        await _dbSet.AddRangeAsync(list);
        await _context.SaveChangesAsync();
        return list.Select(e => _context.Entry(e));
    }

    public async Task<EntityEntry<T>?> UpdateAsync(T entity)
    {
        var entry = _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' not found.");
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id) => await DeleteAsync(Guid.Parse(id));
    public async Task DeleteAsync(T entity) => await DeleteAsync(entity.Id);

    public async Task DeleteListAsync(IEnumerable<Guid> ids)
    {
        var entities = await FindByConditionAsync(e => ids.Contains(e.Id));
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteListAsync(IEnumerable<string> ids)
        => await DeleteListAsync(ids.Select(Guid.Parse));

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(T entity) => await SoftDeleteAsync(entity.Id);
    public async Task SoftDeleteAsync(string id) => await SoftDeleteAsync(Guid.Parse(id));
    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' not found.");
        if (entity is ISoftDeletable soft)
        {
            soft.IsDeleted = true;
            soft.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SoftDeleteListAsync(IEnumerable<T> entities)
    {
        foreach (var e in entities) await SoftDeleteAsync(e.Id);
    }
    public async Task SoftDeleteListAsync(IEnumerable<string> ids)
        => await SoftDeleteListAsync(ids.Select(Guid.Parse));
    public async Task SoftDeleteListAsync(IEnumerable<Guid> ids)
    {
        foreach (var id in ids) await SoftDeleteAsync(id);
    }

    public async Task RestoreAsync(T entity) => await RestoreAsync(entity.Id);
    public async Task RestoreAsync(string id) => await RestoreAsync(Guid.Parse(id));
    public async Task RestoreAsync(Guid id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' not found.");
        if (entity is ISoftDeletable soft)
        {
            soft.IsDeleted = false;
            soft.DeletedAt = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IRepositoryBase<T>.PagedResult<T>> GetPagedAsync(
        int pageNumber, int pageSize)
        => await GetPagedAsync(pageNumber, pageSize, null, null, true, false);

    public async Task<IRepositoryBase<T>.PagedResult<T>> GetPagedAsync(
        int pageNumber, int pageSize,
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var query = includeProperties.Length > 0
            ? FindAll(trackChanges, includeProperties)
            : FindAll(trackChanges);

        if (expression != null)
            query = query.Where(expression);

        if (orderBy != null)
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new IRepositoryBase<T>.PagedResult<T>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
