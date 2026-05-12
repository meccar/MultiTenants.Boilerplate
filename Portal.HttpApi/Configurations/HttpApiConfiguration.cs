using BuildingBlocks.Shared.Helpers;

namespace Host.Configurations;

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
        var apiOptions = configuration.GetSection<ApiOptions>(ApiOptions.SectionName);
        services.Configure<ApiOptions>(options =>
        {
            options.Version = apiOptions.Version;
        });
        services.AddSwaggerConfiguration(configuration);
        services.AddCorsConfiguration(configuration);
        services.AddRateLimitingConfiguration(configuration);
        services.AddHealthCheckConfiguration(configuration);
        services.AddMultiTenantConfiguration();
        services.AddAuthenticationConfiguration(configuration);
        services.AddAuthorizationConfiguration();
        services.AddCarterConfiguration();

        return services;
    }
}
