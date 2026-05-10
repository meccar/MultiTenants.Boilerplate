using BuildingBlocks.Shared.Helpers;
using Microsoft.OpenApi;

namespace Host.Configurations;

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
        var apiVersion = configuration.GetRequiredValue("Api:Version");
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = "Multi-Tenant API",
                Version = apiVersion,
                Description = "Multi-tenant API with CQRS, MongoDB, and OAuth"
            });

            // Add security definition for OAuth
            // OAuth URLs can be overridden via configuration: Authentication:Google:AuthorizationUrl and Authentication:Google:TokenUrl
            var authorizationUrl = ConfigurationHelper.GetRequiredValue(
                    configuration, "Authentication:Google:AuthorizationUrl");
            
            var tokenUrl = ConfigurationHelper.GetRequiredValue(
                    configuration, "Authentication:Google:TokenUrl");

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
