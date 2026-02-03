using Marten;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MultiTenants.Boilerplate.Application.Behaviors;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Application.Stores;
using System.Reflection;
using FluentValidation;

namespace MultiTenants.Boilerplate.Application.Configuration;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
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


