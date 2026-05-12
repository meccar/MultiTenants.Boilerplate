using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Configuration;

/// <summary>
/// Shared layer dependency injection configuration
/// Registers shared utilities, helpers, and cross-cutting concerns
/// </summary>
public static class SharedConfiguration
{
    /// <summary>
    /// Adds shared services to the service collection
    /// This is the main entry point for shared layer service registration
    /// </summary>
    public static IServiceCollection AddShared(
        this IServiceCollection services)
    {
        services.AddSingleton<JwtToken>();

        return services;
    }
}
