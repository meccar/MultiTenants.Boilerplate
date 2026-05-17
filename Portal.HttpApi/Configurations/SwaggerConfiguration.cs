using BuildingBlocks.Shared.Helpers;
using Host.Filters;
using Microsoft.AspNetCore.Authentication.Google;
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
        var apiOptions = configuration.GetSection<ApiOptions>("Api");
        services.AddControllers(options =>
        {
            options.Filters.Add<ApiResponseFilter>(); 
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(apiOptions.Version, new OpenApiInfo
            {
                Title = "Multi-Tenant API",
                Version = apiOptions.Version,
                Description = "Multi-tenant API with CQRS, MongoDB, and OAuth"
            });

            // Add security definition for OAuth
            // OAuth URLs can be overridden via configuration: Authentication:Google:AuthorizationUrl and Authentication:Google:TokenUrl
            var googleOptions = configuration.GetSection<GoogleOptions>("Authentication:Google");

            c.AddSecurityDefinition("Google", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(googleOptions.AuthorizationEndpoint),
                        TokenUrl = new Uri(googleOptions.TokenEndpoint),
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
