using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Marten;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Configurations;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddHealthCheckConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "self" });

        if (services.IsServiceRegistered<IMongoClient>())
        {
            healthChecksBuilder.AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "mongodb", "database", "ready" });
        }

        if (services.IsServiceRegistered<IDocumentStore>())
        {
            var postgresConnectionString = configuration.GetConnectionString("PostgreSQL");
            if (!string.IsNullOrWhiteSpace(postgresConnectionString))
            {
                healthChecksBuilder.AddNpgSql(
                    postgresConnectionString,
                    name: "postgresql",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "postgresql", "database", "ready" });
            }
        }

        return services;
    }
}
