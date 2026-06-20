using Npgsql;
using Stockroom.Infrastructure.Persistence.Data;

namespace Stockroom.Infrastructure.Persistence.Migrations;

public class MigrationRunner
{
    private readonly AppDbContext _context;
    private readonly string _migrationsPath;

    public MigrationRunner(AppDbContext context, string migrationsPath = "Migrations/Scripts")
    {
        _context = context;
        _migrationsPath = migrationsPath;
    }

    public async Task RunAsync()
    {
        await EnsureMigrationsTableExistsAsync();

        var appliedMigrations = await GetAppliedMigrationsAsync();
        var pendingScripts = GetPendingScripts(appliedMigrations);

        foreach (var script in pendingScripts)
        {
            Console.WriteLine($"Applying migration: {script.Name}");
            await ApplyMigrationAsync(script);
        }

        Console.WriteLine($"Migrations complete. {pendingScripts.Count} applied.");
    }

    private async Task EnsureMigrationsTableExistsAsync()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS __migrations_history (
                id              SERIAL PRIMARY KEY,
                migration_name  VARCHAR(255) NOT NULL UNIQUE,
                applied_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            """;

        var conn = await _context.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<HashSet<string>> GetAppliedMigrationsAsync()
    {
        const string sql = "SELECT migration_name FROM __migrations_history;";
        var applied = new HashSet<string>();

        var conn = await _context.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            applied.Add(reader.GetString(0));

        return applied;
    }

    private List<FileInfo> GetPendingScripts(HashSet<string> applied)
    {
        if (!Directory.Exists(_migrationsPath))
            Directory.CreateDirectory(_migrationsPath);

        return Directory.GetFiles(_migrationsPath, "*.sql")
            .Select(f => new FileInfo(f))
            .Where(f => !applied.Contains(f.Name))
            .OrderBy(f => f.Name)
            .ToList();
    }

    private async Task ApplyMigrationAsync(FileInfo script)
    {
        var sql = await File.ReadAllTextAsync(script.FullName);
        var conn = await _context.OpenConnectionAsync();

        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            await using var cmd = new NpgsqlCommand(sql, conn, transaction);
            await cmd.ExecuteNonQueryAsync();

            const string recordSql = """
                INSERT INTO __migrations_history (migration_name) VALUES (@name);
                """;
            await using var recordCmd = new NpgsqlCommand(recordSql, conn, transaction);
            recordCmd.Parameters.AddWithValue("name", script.Name);
            await recordCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}