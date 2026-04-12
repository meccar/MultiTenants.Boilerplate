using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Configurations;

/// <summary>
/// Configuration for authorization services
/// </summary>
public static class AuthorizationConfiguration
{
    /// <summary>
    /// Adds authorization services to the service collection
    /// </summary>
    public static IServiceCollection AddAuthorizationConfiguration(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return services;
    }
}
