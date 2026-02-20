using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MultiTenants.Boilerplate.Infrastructure.Data;

/// <summary>
/// Used by EF Core tools (e.g. dotnet ef migrations) to create AppIdentityDbContext at design time
/// without running the full application and without ITenantProvider.
/// Connection string can be overridden via environment variable ConnectionStrings__DefaultConnection.
/// </summary>
public class DesignTimeAppIdentityDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
{
    public AppIdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=MultiTenantsIdentity;Trusted_Connection=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppIdentityDbContext(optionsBuilder.Options);
    }
}
