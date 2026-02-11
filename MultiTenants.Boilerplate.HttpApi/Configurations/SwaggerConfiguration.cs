using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// Configuration for Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adds Swagger/OpenAPI services to the service collection
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Multi-Tenant API",
                Version = ApiConstants.ApiVersion,
                Description = "Multi-tenant API with CQRS, MongoDB, and OAuth"
            });

            // Add security definition for OAuth
            // OAuth URLs can be overridden via configuration: Authentication:Google:AuthorizationUrl and Authentication:Google:TokenUrl
            var authorizationUrl = configuration["Authentication:Google:AuthorizationUrl"]
                ?? AuthConstants.GoogleAuthorizationUrl;
            var tokenUrl = configuration["Authentication:Google:TokenUrl"]
                ?? AuthConstants.GoogleTokenUrl;

            c.AddSecurityDefinition("Google", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationUrl),
                        TokenUrl = new Uri(tokenUrl),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "profile", "Profile" },
                            { "email", "Email" }
                        }
                    }
                }
            });
        });

        return services;
    }
}
