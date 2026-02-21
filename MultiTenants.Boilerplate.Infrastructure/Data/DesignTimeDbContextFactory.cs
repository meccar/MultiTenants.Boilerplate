using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Infrastructure.Data;

/// <summary>
/// Used by EF Core tools (e.g. dotnet ef migrations) to create AppDbContext at design time.
/// Connection string: env ConnectionStrings__DefaultConnection, or <see cref="DatabaseProviderHelper.GetDefaultConnectionString"/>.
/// Provider: env Database__Provider, or inferred from connection string via <see cref="DatabaseProviderHelper.DetectProviderFromConnectionString"/>.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? DatabaseProviderHelper.GetDefaultConnectionString(DatabaseProviderHelper.PostgreSQL);

        var provider = Environment.GetEnvironmentVariable("Database__Provider")
            ?? DatabaseProviderHelper.DetectProviderFromConnectionString(connectionString)
            ?? DatabaseProviderHelper.SqlServer;

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        ConfigureProvider(optionsBuilder, provider, connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static void ConfigureProvider(DbContextOptionsBuilder<AppDbContext> optionsBuilder, string provider, string connectionString)
    {
        if (string.Equals(provider, DatabaseProviderHelper.PostgreSQL, StringComparison.OrdinalIgnoreCase))
            optionsBuilder.UseNpgsql(connectionString);
        else if (string.Equals(provider, DatabaseProviderHelper.MySQL, StringComparison.OrdinalIgnoreCase))
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        else
            optionsBuilder.UseSqlServer(connectionString);
    }
}
