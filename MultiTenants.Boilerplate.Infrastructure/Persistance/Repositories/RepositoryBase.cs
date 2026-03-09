using Microsoft.EntityFrameworkCore.ChangeTracking;
using MultiTenants.Boilerplate.Domain.Seedwork.Entity;
using MultiTenants.Boilerplate.Domain.Seedwork.Interface;
using System.Data.Common;
using System.Linq.Expressions;

namespace MultiTenants.Boilerplate.Infrastructure.Persistance.Repositories;
public class RepositoryBase<T> 
    : IRepositoryBase<T> where T : EntityBase, new()
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly string _tableName;

    public RepositoryBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tableName = typeof(T).Name;
    }

    public async Task<int> CountAsync()
    {
        var cmd = await CreateCommandAsync();
        cmd.CommandText = $"SELECT COUNT(*) FROM {_tableName}";

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<EntityEntry<T>?> CreateAsync(T entity)
    {
        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id");

        var columns = string .Join(", ", props.Select(p => p.Name));
        var parameters = string.Join(", ", props.Select(p => $"@{p.Name}"));

        var cmd = await CreateCommandAsync();

        cmd.CommandText = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        await cmd.ExecuteNonQueryAsync();
        return null;
    }

    public async Task<IEnumerable<EntityEntry<T>>> CreateListAsync(
        IEnumerable<T> entities)
    {
        foreach (var e in entities)
            await CreateAsync(e);

        return Enumerable.Empty<EntityEntry<T>>();
    }


    public async Task DeleteAsync(Guid id)
    {
        var cmd = await CreateCommandAsync();

        cmd.CommandText = $"DELETE FROM {_tableName} WHERE Id = @Id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@Id";
        param.Value = id;

        cmd.Parameters.Add(param);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string id)
        => await DeleteAsync(Guid.Parse(id));

    public async Task DeleteAsync(T entity)
        => await DeleteAsync(entity.Id);

    public async Task DeleteListAsync(IEnumerable<Guid> ids)
    {
        foreach (var id in ids)
            await DeleteAsync(id);
    }
    public async Task DeleteListAsync(IEnumerable<string> ids)
        => await DeleteListAsync(ids.Select(Guid.Parse));

    public async Task DeleteListAsync(IEnumerable<T> entities)
        => await DeleteListAsync(entities.Select(e => e.Id));

    public async Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false)
    {
        var cmd = await CreateCommandAsync();
        cmd.CommandText = $"SELECT * FROM {_tableName}";

        var list = new List<T>();

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var cmd = await CreateCommandAsync();
        cmd.CommandText = $"SELECT * FROM {_tableName} WHERE Id = @Id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@Id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.Read())
            return null;

        return MapEntity(reader);
    }
    public Task<T?> GetByIdAsync(string id)
        => GetByIdAsync(Guid.Parse(id));

    public async Task<bool> ExistAsync(string id)
    => await GetByIdAsync(id) != null;

    public async Task<bool> ExistAsync(Guid id)
        => await GetByIdAsync(id) != null;

    public async Task<EntityEntry<T>?> UpdateAsync(T entity)
    {
        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id");

        var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

        var cmd = await CreateCommandAsync();

        cmd.CommandText = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        await cmd.ExecuteNonQueryAsync();
        return null;
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        foreach (var e in entities)
            await UpdateAsync(e);
    }

    private async Task<DbCommand> CreateCommandAsync()
    {
        if (_unitOfWork.Connection.State != System.Data.ConnectionState.Open)
            await _unitOfWork.Connection.OpenAsync();

        var cmd = _unitOfWork.Connection.CreateCommand();
        cmd.Transaction = _unitOfWork.Transaction;
        return cmd;
    }

    protected virtual T MapEntity(DbDataReader reader)
    {
        var entity = new T();

        foreach (var prop in typeof(T).GetProperties())
        {
            if (!HasColumn(reader, prop.Name))
                continue;

            var value = reader[prop.Name];
            if (value == DBNull.Value)
                continue;

            prop.SetValue(entity, value);
        }

        return entity;
    }

    private bool HasColumn(DbDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i)
                .Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public Task SoftDeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteListAsync(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteListAsync(IEnumerable<string> ids)
    {
        throw new NotImplementedException();
    }

    public Task SoftDeleteListAsync(IEnumerable<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IRepositoryBase<T>.PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<IRepositoryBase<T>.PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>? orderBy = null, bool ascending = true, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindAll(bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindAllAsync(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }
}
