using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// HttpApi layer dependency injection configuration
/// Orchestrates all HttpApi layer service registrations
/// Acts as a unit of work for HttpApi service configuration
/// </summary>
public static class HttpApiConfiguration
{
    /// <summary>
    /// Adds all HttpApi layer services to the service collection
    /// This is the main entry point for HttpApi layer service registration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHttpApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // API version and base path (from Api:Version in appsettings)
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));

        // API Documentation
        services.AddSwaggerConfiguration(configuration);

        // Cross-Origin Resource Sharing
        services.AddCorsConfiguration(configuration);

        // Rate Limiting
        services.AddRateLimitingConfiguration(configuration);

        // Health Checks
        services.AddHealthCheckConfiguration(configuration);

        // OAuth
        services.AddOAuthConfiguration(configuration);

        // Multi-Tenancy
        services.AddMultiTenantConfiguration();

        // Authorization
        services.AddAuthorizationConfiguration();

        // Carter Endpoints
        services.AddCarterConfiguration();

        return services;
    }
}
