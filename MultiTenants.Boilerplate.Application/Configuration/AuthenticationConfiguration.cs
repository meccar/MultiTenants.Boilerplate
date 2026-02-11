using Microsoft.Extensions.DependencyInjection;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration for authentication services
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Configures authentication cookie options
    /// </summary>
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/api/auth/login/google";
            options.LogoutPath = "/api/auth/logout";
            options.AccessDeniedPath = "/access-denied";
        });

        return services;
    }
}
