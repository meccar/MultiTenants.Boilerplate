using Identity.Application.Handlers.PermissionRequirement;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Configurations.Auth;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Own DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb")));

        // ASP.NET Identity
        services.AddIdentity<UsersEntity, RolesEntity>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // RBAC services
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Authorization handler
        services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();

        return services;
    }
}
