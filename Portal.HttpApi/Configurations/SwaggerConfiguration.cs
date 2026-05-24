using BuildingBlocks.Shared.Helpers;
using Host.Filters;
using Microsoft.OpenApi;

namespace Host.Configurations;

public static class SwaggerConfiguration
{
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

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. The 'Bearer ' prefix is added automatically."
            });

            c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    []
                }
            });

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