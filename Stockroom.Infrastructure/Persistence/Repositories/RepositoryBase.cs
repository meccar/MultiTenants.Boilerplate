using BuildingBlocks.Core.Seedwork.Interface;
using Npgsql;
using Stockroom.Infrastructure.Persistence.Data;

namespace Stockroom.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class, new()
{
    private readonly AppDbContext _context;

    protected abstract string TableName { get; }
    protected abstract string PrimaryKeyColumn { get; }
    protected abstract TKey GetPrimaryKey(TEntity entity);
    protected abstract void SetPrimaryKey(TEntity entity, TKey id);
    protected abstract TEntity Map(NpgsqlDataReader reader);
    protected abstract IEnumerable<NpgsqlParameter> GetParameters(TEntity entity);
    protected abstract string GetInsertSql();
    protected abstract string GetUpdateSql();
    protected virtual bool SupportsSoftDelete => false;
    protected virtual string SoftDeleteColumn => "is_deleted";
    protected virtual string DeletedAtColumn => "deleted_at";

    protected RepositoryBase(AppDbContext context)
        => _context = context ?? throw new ArgumentNullException(nameof(context));

    protected NpgsqlCommand CreateCommand(string sql, CancellationToken ct = default)
    {
        var cmd = new NpgsqlCommand(sql, _context.Connection, _context.CurrentTransaction);
        cmd.CommandTimeout = 30;
        return cmd;
    }

    protected async Task<IEnumerable<TEntity>> QueryAsync(
        string sql,
        IEnumerable<NpgsqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        await _context.OpenConnectionAsync();
        await using var cmd = CreateCommand(sql, ct);

        if (parameters != null)
            cmd.Parameters.AddRange(parameters.ToArray());

        var results = new List<TEntity>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            results.Add(Map(reader));

        return results;
    }

    protected async Task<TEntity?> QuerySingleAsync(
        string sql,
        IEnumerable<NpgsqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        var results = await QueryAsync(sql, parameters, ct);
        return results.FirstOrDefault();
    }

    protected async Task<int> ExecuteAsync(
        string sql,
        IEnumerable<NpgsqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        await _context.OpenConnectionAsync();
        await using var cmd = CreateCommand(sql, ct);

        if (parameters != null)
            cmd.Parameters.AddRange(parameters.ToArray());

        var rows = await cmd.ExecuteNonQueryAsync(ct);
        _context.TrackChange(rows);
        return rows;
    }

    protected async Task<T?> ExecuteScalarAsync<T>(
        string sql,
        IEnumerable<NpgsqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        await _context.OpenConnectionAsync();
        await using var cmd = CreateCommand(sql, ct);

        if (parameters != null)
            cmd.Parameters.AddRange(parameters.ToArray());

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is DBNull or null ? default : (T)result;
    }
    
    public async Task<TEntity?> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        var id = await ExecuteScalarAsync<TKey>(GetInsertSql(), GetParameters(entity), ct);
        if (id != null) SetPrimaryKey(entity, id);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> CreateListAsync(
        IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        var results = new List<TEntity>();
        foreach (var entity in entities)
        {
            var created = await CreateAsync(entity, ct);
            if (created != null) results.Add(created);
        }
        return results;
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        await ExecuteAsync(GetUpdateSql(), GetParameters(entity), ct);
        return entity;
    }

    public async Task UpdateListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
            await UpdateAsync(entity, ct); // ← ct now passed
    }

    // -------------------------------------------------------------------------
    // Delete (hard)
    // -------------------------------------------------------------------------

    public async Task DeleteAsync(TKey id, CancellationToken ct = default)
        => await ExecuteAsync(
            $"DELETE FROM {TableName} WHERE {PrimaryKeyColumn} = @id;",
            [new NpgsqlParameter("id", id)], ct);

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
        => await DeleteAsync(GetPrimaryKey(entity), ct);

    public async Task DeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
            await DeleteAsync(entity, ct);
    }

    public async Task DeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        foreach (var id in ids)
            await DeleteAsync(id, ct);
    }
    
    public async Task SoftDeleteAsync(TKey id, CancellationToken ct = default)
    {
        if (!SupportsSoftDelete)
            throw new NotSupportedException($"{TableName} does not support soft delete.");

        await ExecuteAsync($"""
            UPDATE {TableName}
            SET {SoftDeleteColumn} = TRUE, {DeletedAtColumn} = NOW()
            WHERE {PrimaryKeyColumn} = @id;
            """, [new NpgsqlParameter("id", id)], ct);
    }

    public async Task SoftDeleteAsync(TEntity entity, CancellationToken ct = default)
        => await SoftDeleteAsync(GetPrimaryKey(entity), ct); // ← was calling itself

    public async Task SoftDeleteListAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        foreach (var id in ids)
            await SoftDeleteAsync(id, ct);
    }

    public async Task SoftDeleteListAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
            await SoftDeleteAsync(entity, ct);
    }

    public async Task RestoreAsync(TKey id, CancellationToken ct = default)
    {
        if (!SupportsSoftDelete)
            throw new NotSupportedException($"{TableName} does not support restore.");

        await ExecuteAsync($"""
            UPDATE {TableName}
            SET {SoftDeleteColumn} = FALSE, {DeletedAtColumn} = NULL
            WHERE {PrimaryKeyColumn} = @id;
            """, [new NpgsqlParameter("id", id)], ct);
    }

    public async Task RestoreAsync(TEntity entity, CancellationToken ct = default)
        => await RestoreAsync(GetPrimaryKey(entity), ct);
    
    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await QuerySingleAsync(
            $"SELECT * FROM {TableName} WHERE {PrimaryKeyColumn} = @id;",
            [new NpgsqlParameter("id", id)], ct);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var sql = SupportsSoftDelete
            ? $"SELECT * FROM {TableName} WHERE {SoftDeleteColumn} = FALSE;"
            : $"SELECT * FROM {TableName};";

        return await QueryAsync(sql, ct: ct);
    }

    public async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default)
    {
        var count = await ExecuteScalarAsync<long>(
            $"SELECT COUNT(1) FROM {TableName} WHERE {PrimaryKeyColumn} = @id;",
            [new NpgsqlParameter("id", id)], ct);
        return count > 0;
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        var sql = SupportsSoftDelete
            ? $"SELECT COUNT(1) FROM {TableName} " +
              $"WHERE {SoftDeleteColumn} = FALSE;"
            : $"SELECT COUNT(1) FROM {TableName};";

        var count = await ExecuteScalarAsync<long>(sql, ct: ct);
        return (int)count;
    }

    public async Task<IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var totalCount = await CountAsync(ct);

        var sql = $"""
            SELECT * FROM {TableName}
            {(SupportsSoftDelete ? $"WHERE {SoftDeleteColumn} = FALSE" : "")}
            ORDER BY {PrimaryKeyColumn}
            LIMIT @limit OFFSET @offset;
            """;

        var items = await QueryAsync(sql, [
            new NpgsqlParameter("limit",  pageSize),
            new NpgsqlParameter("offset", (pageNumber - 1) * pageSize)
        ], ct);

        return new IRepositoryBase<TEntity, TKey>.PagedResult<TEntity>
        {
            Items      = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize   = pageSize
        };
    }
}
