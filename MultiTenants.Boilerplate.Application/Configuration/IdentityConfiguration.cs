using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Stores;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration for ASP.NET Identity services
/// </summary>
public static class IdentityConfiguration
{
    /// <summary>
    /// Adds ASP.NET Identity services to the service collection
    /// </summary>
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddDefaultTokenProviders();

        services.AddScoped<IUserStore<IdentityUser>, MongoUserStore>();
        services.AddScoped<IRoleStore<IdentityRole>, MongoRoleStore>();

        return services;
    }
}
