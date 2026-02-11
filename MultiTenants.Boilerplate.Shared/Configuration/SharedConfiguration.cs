using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Shared.Configuration;

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
        // Shared utilities and cross-cutting services
        // Currently, shared layer contains constants, utilities, and responses
        // Add shared services as needed:
        // services.AddScoped<ISharedService, SharedService>();

        return services;
    }
}
