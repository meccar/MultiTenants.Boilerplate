using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Application.Services;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Application layer dependency injection configuration
/// Orchestrates all application layer service registrations
/// Acts as a unit of work for application service configuration
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Adds all application layer services to the service collection
    /// This is the main entry point for application layer service registration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Core CQRS and Validation
        services.AddCQRS();

        // HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Database Services
        services.AddMongoDb(configuration);
        services.AddPostgreSQL(configuration);

        // Email sender for auth endpoints (confirmation, password reset emails)
        services.AddScoped<IEmailSender, LoggingEmailSender>();

        // JWT token generation for auth command handlers (LocalAuthentication, OAuthAuthentication)
        services.AddScoped<JwtToken>();

        // Optional Services (only registered if configured)
        services.AddMessageBroker(configuration);
        services.AddCaching(configuration);

        return services;
    }
}


