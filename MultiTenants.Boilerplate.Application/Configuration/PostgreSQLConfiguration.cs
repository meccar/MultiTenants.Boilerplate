using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration for PostgreSQL/Marten services
/// </summary>
public static class PostgreSQLConfiguration
{
    /// <summary>
    /// Adds PostgreSQL/Marten services to the service collection
    /// </summary>
    public static IServiceCollection AddPostgreSQL(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("PostgreSQL")
            ?? "Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres";

        services.AddMarten(options =>
        {
            options.Connection(postgresConnectionString);
        });

        return services;
    }
}
