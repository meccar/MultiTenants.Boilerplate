using Identity.Application.Behaviors;
using Identity.Application.Mapper;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
            
            services.AddAutoMapper(option =>
            {
                option.CreateMap<AppUser, IdentityUser>();
            });
            return services;
        }
    }
}
