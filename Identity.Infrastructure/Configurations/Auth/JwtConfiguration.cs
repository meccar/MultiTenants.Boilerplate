using System.Security.Claims;
using System.Text;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Configurations.Auth;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                    ValidateIssuer   = true,
                    ValidIssuer      = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience    = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew        = TimeSpan.Zero,
                    // Map "sub" → ClaimTypes.NameIdentifier automatically
                    NameClaimType    = ClaimTypes.NameIdentifier,
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var permissionService = context.HttpContext.RequestServices
                            .GetRequiredService<IPermissionService>();

                        var userId = Guid.Parse(
                            context.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!);
                        var tenantId = context.Principal.FindFirstValue("tenant_id") is { } t
                            ? Guid.Parse(t) : (Guid?)null;

                        var (roles, permissions) = await permissionService
                            .GetUserRolesAndPermissionsAsync(userId, tenantId);

                        var identity = (ClaimsIdentity)context.Principal.Identity!;
                        identity.AddClaims(roles.Select(r =>
                            new Claim(ClaimTypes.Role, r)));
                        identity.AddClaims(permissions.Select(p =>
                            new Claim("permission", p)));
                    },

                    OnAuthenticationFailed = context =>
                    {
                        // Normalize the 401 response
                        context.Response.Headers["WWW-Authenticate"] =
                            "Bearer error=\"invalid_token\"";
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}