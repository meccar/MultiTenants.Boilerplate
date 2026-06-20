using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Stockroom.Infrastructure.Persistence.Data;

public class AppDbContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _currentTransaction;

    public AppDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public NpgsqlConnection Connection
    {
        get
        {
            if (_connection == null)
                _connection = new NpgsqlConnection(_connectionString);
            return _connection;
        }
    }

    public NpgsqlTransaction? CurrentTransaction => _currentTransaction;

    public async Task<NpgsqlConnection> OpenConnectionAsync()
    {
        if (Connection.State != System.Data.ConnectionState.Open)
            await Connection.OpenAsync();
        return Connection;
    }

    public async Task<NpgsqlTransaction> BeginTransactionAsync()
    {
        await OpenConnectionAsync();
        _currentTransaction = await Connection.BeginTransactionAsync();
        return _currentTransaction;
    }

    public async Task CommitAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _connection?.Dispose();
    }
}