using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Stockroom.Infrastructure.Persistence.Data;

public class AppDbContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _currentTransaction;
    private int _pendingChanges = 0;

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

    public void TrackChange(int rowsAffected)
        => Interlocked.Add(ref _pendingChanges, rowsAffected);
    
    public NpgsqlTransaction? CurrentTransaction => _currentTransaction;

    public async Task<NpgsqlConnection> OpenConnectionAsync()
    {
        if (Connection.State != System.Data.ConnectionState.Open)
            await Connection.OpenAsync();
        return Connection;
    }

    public async Task<NpgsqlTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        await OpenConnectionAsync();
        _currentTransaction = await Connection.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        var saved = Interlocked.Exchange(ref _pendingChanges, 0);
        return saved;
    }
    
    public async Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _connection?.Dispose();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}