using BuildingBlocks.Shared.Constants;
using BuildingBlocks.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.Google;

namespace Host.Configurations;

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
        var googleOption = configuration.GetSection<GoogleOptions>("Authentication:Google");
        
        services.AddAuthentication()
            .AddGoogle(AuthConstants.GoogleScheme, options =>
            {
                options.ClientId = googleOption.ClientId;
                options.ClientSecret = googleOption.ClientSecret;
                options.SignInScheme = AuthConstants.DefaultScheme;
                options.CallbackPath = $"/api/{configuration["Api:Version"]?.Trim() ?? "v1"}/auth/login/google/callback";
            });

        return services;
    }
}
