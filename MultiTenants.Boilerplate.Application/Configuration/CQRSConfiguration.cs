using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MultiTenants.Boilerplate.Application.Behaviors;
using System.Reflection;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration for CQRS (MediatR) and validation services
/// </summary>
public static class CQRSConfiguration
{
    /// <summary>
    /// Adds MediatR and FluentValidation services to the service collection
    /// </summary>
    public static IServiceCollection AddCQRS(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
