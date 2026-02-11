using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// Configuration for OAuth services (Google OAuth)
/// </summary>
public static class OAuthConfiguration
{
    /// <summary>
    /// Adds OAuth authentication services to the service collection
    /// </summary>
    public static IServiceCollection AddOAuthConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Retrieve and validate Google OAuth credentials
        var googleClientId = configuration["Authentication:Google:ClientId"];
        var googleClientSecret = configuration["Authentication:Google:ClientSecret"];

        // Validate ClientId - fail fast with clear error message
        if (string.IsNullOrWhiteSpace(googleClientId) ||
            googleClientId.Equals("YOUR_GOOGLE_CLIENT_ID", StringComparison.OrdinalIgnoreCase))
        {
            var envVarName = "Authentication__Google__ClientId";
            throw new InvalidOperationException(
                $"Google OAuth ClientId is required but not configured. " +
                $"Please set the environment variable '{envVarName}' or configure it via User Secrets. " +
                $"For development, use: dotnet user-secrets set \"Authentication:Google:ClientId\" \"your-client-id\"");
        }

        // Validate ClientSecret - fail fast with clear error message
        if (string.IsNullOrWhiteSpace(googleClientSecret) ||
            googleClientSecret.Equals("YOUR_GOOGLE_CLIENT_SECRET", StringComparison.OrdinalIgnoreCase))
        {
            var envVarName = "Authentication__Google__ClientSecret";
            throw new InvalidOperationException(
                $"Google OAuth ClientSecret is required but not configured. " +
                $"Please set the environment variable '{envVarName}' or configure it via User Secrets. " +
                $"For development, use: dotnet user-secrets set \"Authentication:Google:ClientSecret\" \"your-client-secret\"");
        }

        services.AddAuthentication()
            .AddGoogle(AuthConstants.GoogleScheme, options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.SignInScheme = AuthConstants.DefaultScheme;
                options.CallbackPath = $"{ApiConstants.ApiBasePath}/auth/login/google/callback";
            });

        return services;
    }
}
