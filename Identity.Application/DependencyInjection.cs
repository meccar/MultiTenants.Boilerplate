using Identity.Application.Behaviors;
using Identity.Application.Services;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureIdentityApplicationDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped(
                typeof(IPipelineBehavior<,>), 
                typeof(PermissionValidationBehavior<,>));
            services.AddScoped<ICurrentUser, CurrentUserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            return services;
        }
    }
}
