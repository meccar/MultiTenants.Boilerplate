using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Stores;
using MultiTenants.Boilerplate.Application.Helpers;
using MongoDB.Driver;
using Marten;

namespace MultiTenants.Boilerplate.Application.Configuration;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddHttpContextAccessor();

        var mongoConnectionString = configuration.GetRequiredConfigurationValue("MongoDB");

        var mongoClient = new MongoClient(mongoConnectionString);
        var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName") ?? "multitenants";
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped(_ => mongoClient.GetDatabase(databaseName));

        var postgresConnectionString = configuration.GetConnectionString("PostgreSQL")
            ?? "Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres";

        services.AddMarten(options =>
        {
            options.Connection(postgresConnectionString);
            // Note: If you need to store IdentityUser in Marten, configure it here
            // For now, Identity is stored in MongoDB
        });

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

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/api/auth/login/google";
            options.LogoutPath = "/api/auth/logout";
            options.AccessDeniedPath = "/access-denied";
        });

        return services;
    }
}


