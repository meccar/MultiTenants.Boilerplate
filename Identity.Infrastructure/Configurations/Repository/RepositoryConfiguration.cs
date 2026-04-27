using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Configurations.Repository;

public static class RepositoryConfiguration
{
    public static IServiceCollection AddRepositoryConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IGroupsRepository, GroupRepository>();
        services.AddScoped<IPermissionsRepository, PermissionsRepository>();
        services.AddScoped<IGroupRoleRepository, GroupRoleRepository>();
        services.AddScoped<IPoliciesRepository, PoliciesRepository>();
        services.AddScoped<IPolicyPermissionRepository, PolicyPermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRolePolicyRepository, RolePolicyRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        
        return services;
    }
}