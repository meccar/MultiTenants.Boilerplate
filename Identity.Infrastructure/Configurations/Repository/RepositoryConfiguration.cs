using Identity.Domain.Interfaces;
using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Infrastructure.Persistence.Repositories;
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
        services.AddScoped<IGroupPolicyRepository, GroupPolicyRepository>();
        services.AddScoped<IPoliciesRepository, PoliciesRepository>();
        services.AddScoped<IPolicyPermissionRepository, PolicyPermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRolePolicyRepository, RolePolicyRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IUserPolicyRepository, UserPolicyRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
