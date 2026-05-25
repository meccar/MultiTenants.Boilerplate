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
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrWhiteSpace(authHeader) && 
                            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader["Bearer ".Length..].Trim();
                            Console.WriteLine($"[JWT] Manually extracted token: '{context.Token[..20]}...'");
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT] Auth FAILED: {context.Exception.GetType().Name}: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"[JWT] Token VALIDATED for: {context.Principal?.Identity?.Name}");
                        Console.WriteLine($"[JWT] Claims: {string.Join(", ", context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? [])}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        Console.WriteLine($"[JWT] Challenge triggered. Error: {context.Error}, ErrorDescription: {context.ErrorDescription}");
                        context.HandleResponse();
                        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Unauthorized());
                    },
                    OnForbidden = async context =>
                    {
                        await context.Response.WriteAsJsonAsync(ApiResponse<object>.FailureResponse());
                    }
                };
            });

        return services;
    }
}
