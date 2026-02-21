namespace MultiTenants.Boilerplate.Shared.Utilities;

/// <summary>
/// Reusable helpers for database connection strings and provider detection.
/// Used by Infrastructure (runtime) and design-time EF tools (migrations).
/// </summary>
public static class DatabaseProviderHelper
{
    /// <summary>Provider key for PostgreSQL.</summary>
    public const string PostgreSQL = "PostgreSQL";

    /// <summary>Provider key for MySQL.</summary>
    public const string MySQL = "MySQL";

    /// <summary>Provider key for SQL Server (default when detection returns null).</summary>
    public const string SqlServer = "SqlServer";

    /// <summary>
    /// Infers the database provider from a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string (can be null or empty).</param>
    /// <returns>One of <see cref="PostgreSQL"/>, <see cref="MySQL"/>, or null (treat as SQL Server).</returns>
    public static string? DetectProviderFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return null;
        var s = connectionString.Trim();
        if (s.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
            || s.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            return PostgreSQL;
        if (s.StartsWith("Host=", StringComparison.OrdinalIgnoreCase))
            return PostgreSQL;
        if (s.StartsWith("Server=", StringComparison.OrdinalIgnoreCase)
            && (s.Contains("mysql", StringComparison.OrdinalIgnoreCase) || s.Contains("3306", StringComparison.Ordinal)))
            return MySQL;
        return null;
    }

    /// <summary>
    /// Returns a default connection string for the given provider (e.g. local dev).
    /// </summary>
    /// <param name="provider">One of <see cref="PostgreSQL"/>, <see cref="MySQL"/>, or anything else (SQL Server).</param>
    /// <param name="databaseName">Optional database name; defaults to "MultiTenantsIdentity".</param>
    public static string GetDefaultConnectionString(string provider, string databaseName = "MultiTenantsIdentity")
    {
        var p = provider?.Trim() ?? string.Empty;
        if (string.Equals(p, PostgreSQL, StringComparison.OrdinalIgnoreCase))
            return $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
        if (string.Equals(p, MySQL, StringComparison.OrdinalIgnoreCase))
            return $"Server=localhost;Port=3306;Database={databaseName};User=root;Password=;";
        return "Server=(localdb)\\mssqllocaldb;Database=" + databaseName + ";Trusted_Connection=True;MultipleActiveResultSets=true";
    }
}
