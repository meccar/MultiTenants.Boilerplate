using Carter;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// Configuration for Carter endpoint routing
/// </summary>
public static class CarterConfiguration
{
    /// <summary>
    /// Adds Carter services to the service collection
    /// </summary>
    public static IServiceCollection AddCarterConfiguration(
        this IServiceCollection services)
    {
        services.AddCarter();

        return services;
    }
}
