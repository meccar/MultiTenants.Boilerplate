using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Shared.Utilities;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Checks if a service of type T is registered in the service collection
    /// </summary>
    /// <typeparam name="T">The service type to check</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>True if the service is registered, false otherwise</returns>
    public static bool IsServiceRegistered<T>(this IServiceCollection services)
    {
        return services.Any(s => s.ServiceType == typeof(T));
    }
}
