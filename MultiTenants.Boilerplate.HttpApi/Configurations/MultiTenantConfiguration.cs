using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// Configuration for multi-tenant services
/// </summary>
public static class MultiTenantConfiguration
{
    /// <summary>
    /// Adds multi-tenant services to the service collection
    /// </summary>
    public static IServiceCollection AddMultiTenantConfiguration(
        this IServiceCollection services)
    {
        services.AddMultiTenant<TenantInfo>()
            .WithRouteStrategy()
            .WithInMemoryStore();

        return services;
    }
}
