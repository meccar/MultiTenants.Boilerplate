using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Infrastructure.Data;
using MultiTenants.Boilerplate.Infrastructure.Identity;
using MultiTenants.Boilerplate.Infrastructure.Services;
using MultiTenants.Boilerplate.Infrastructure.Tenancy;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace MultiTenants.Boilerplate.Infrastructure.Configuration;

/// <summary>
/// Infrastructure layer configuration entry point.
/// Orchestrates Infrastructure service registrations (Identity, EF Core, IIdentityService).
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// Database provider is selected via Database:Provider or DatabaseProvider
    /// (SqlServer | PostgreSQL | MySQL); default is SqlServer. Connection string from ConnectionStrings:DefaultConnection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Tenant resolution (used by DbContext and IdentityService)
        services.AddScoped<ITenantProvider, TenantProvider>();

        // DbContext and database provider: read Database:Provider or DatabaseProvider; default SqlServer
        var provider = configuration["Database:Provider"] ?? configuration["DatabaseProvider"] ?? "SqlServer";
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? GetDefaultConnectionString(provider);

        var providerKey = provider.Trim();
        services.AddDbContext<AppIdentityDbContext>(options =>
        {
            if (string.Equals(providerKey, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
                options.UseNpgsql(connectionString);
            else if (string.Equals(providerKey, "MySQL", StringComparison.OrdinalIgnoreCase))
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            else
                options.UseSqlServer(connectionString);
        });

        // ASP.NET Core Identity with AppUser/AppRole
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        // Email sender for Identity (confirmation, password reset)
        services.AddScoped<IEmailSender<AppUser>, LoggingIdentityEmailSender>();

        // Tenant-aware identity service for Application layer
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    private static string GetDefaultConnectionString(string provider)
    {
        if (string.Equals(provider.Trim(), "PostgreSQL", StringComparison.OrdinalIgnoreCase))
            return "Host=localhost;Port=5432;Database=MultiTenantsIdentity;Username=postgres;Password=postgres";
        if (string.Equals(provider.Trim(), "MySQL", StringComparison.OrdinalIgnoreCase))
            return "Server=localhost;Port=3306;Database=MultiTenantsIdentity;User=root;Password=;";
        return "Server=(localdb)\\mssqllocaldb;Database=MultiTenantsIdentity;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
}
