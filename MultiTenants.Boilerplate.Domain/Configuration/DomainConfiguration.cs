using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Domain.Configuration;

/// <summary>
/// Domain layer dependency injection configuration
/// Registers domain services, repositories, and domain-specific services
/// </summary>
public static class DomainConfiguration
{
    /// <summary>
    /// Adds domain services to the service collection
    /// This is the main entry point for domain layer service registration
    /// </summary>
    public static IServiceCollection AddDomain(
        this IServiceCollection services)
    {
        // Domain services, repositories, and domain-specific registrations go here
        // Currently, domain layer is primarily entities and value objects
        // Add domain services as needed:
        // services.AddScoped<IDomainService, DomainService>();

        return services;
    }
}
