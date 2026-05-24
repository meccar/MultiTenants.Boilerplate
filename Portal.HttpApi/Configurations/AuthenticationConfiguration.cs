using System.Text;
using BuildingBlocks.Shared.Configuration;
using BuildingBlocks.Shared.Helpers;
using BuildingBlocks.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Host.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection<JwtOptions>("Jwt");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object>.Unauthorized());
                    },
                    OnForbidden = async context =>
                    {
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object>.FailureResponse()
                        );
                    }
                };
            });

        return services;
    }
}
