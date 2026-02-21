using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Infrastructure.Data;
using MultiTenants.Boilerplate.Infrastructure.Identity;
using MultiTenants.Boilerplate.Infrastructure.Services;
using MultiTenants.Boilerplate.Infrastructure.Helpers;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Infrastructure.Configuration;

/// <summary>
/// Infrastructure layer configuration entry point.
/// Orchestrates Infrastructure service registrations (Identity, EF Core, IIdentityService).
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    /// <remarks>
    /// <para><b>Recommended:</b> Set provider explicitly in appsettings for clarity and reliability:</para>
    /// <code>
    /// "Database": { "Provider": "PostgreSQL" },
    /// "ConnectionStrings": { "DefaultConnection": "Host=...;Database=...;Username=...;Password=...;" }
    /// </code>
    /// <para>If Database:Provider (or DatabaseProvider) is not set, provider is inferred from the connection string
    /// (postgresql://, Host=, or Server= with mysql/3306). Default when unspecified is SqlServer.</para>
    /// </remarks>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Tenant resolution (used by DbContext and IdentityService)
        services.AddScoped<ITenantProvider, TenantHelper>();

        // DbContext and database provider: explicit Database:Provider, or auto-detect from connection string via Shared helper
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var provider = configuration["Database:Provider"] ?? configuration["DatabaseProvider"];
        if (string.IsNullOrWhiteSpace(provider) && !string.IsNullOrWhiteSpace(connectionString))
            provider = DatabaseProviderHelper.DetectProviderFromConnectionString(connectionString);
        provider ??= DatabaseProviderHelper.SqlServer;
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = DatabaseProviderHelper.GetDefaultConnectionString(provider);

        var providerKey = provider.Trim();
        services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            if (string.Equals(providerKey, DatabaseProviderHelper.PostgreSQL, StringComparison.OrdinalIgnoreCase))
                options.UseNpgsql(connectionString);
            else if (string.Equals(providerKey, DatabaseProviderHelper.MySQL, StringComparison.OrdinalIgnoreCase))
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Email sender for Identity (confirmation, password reset)
        services.AddScoped<IEmailSender<AppUser>, LoggingIdentityEmailSender>();

        // Tenant-aware identity service for Application layer
        services.AddScoped<IIdentityService, IdentityService>();

        // Data seeder for default tenant (TAF), Admin role, and admin user (idempotent)
        services.AddScoped<DataSeeder>();

        return services;
    }

}
