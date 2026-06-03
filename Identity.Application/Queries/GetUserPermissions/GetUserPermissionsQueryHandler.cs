using BuildingBlocks.Shared.Exceptions;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, CurrentUserModel>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserPolicyRepository _userPolicyRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IGroupPolicyRepository _groupPolicyRepository;
    private readonly IPolicyPermissionRepository _policyPermissionRepository;
    
    
    public GetUserPermissionsQueryHandler(
        UserManager<UsersEntity> userManager,
        IRolePermissionRepository rolePermissionRepository,
        IUserPolicyRepository userPolicyRepository,
        IUserGroupRepository userGroupRepository,
        IGroupPolicyRepository groupPolicyRepository,
        IPolicyPermissionRepository policyPermissionRepository
    )
    {
        _userManager = userManager;
        _rolePermissionRepository = rolePermissionRepository;
        _userPolicyRepository = userPolicyRepository;
        _userGroupRepository = userGroupRepository;
        _groupPolicyRepository = groupPolicyRepository;
        _policyPermissionRepository = policyPermissionRepository;
    }

    public async Task<CurrentUserModel> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new UnauthorizedException();
        
        var user = await _userManager.FindByNameAsync(request.Username)
                   ?? await _userManager.FindByEmailAsync(request.Username);
        
        if (user is null)
            throw new UnauthorizedException();

        List<PoliciesEntity> policies = [];
        
        var userRoleNames = await _userManager.GetRolesAsync(user);
        if (userRoleNames.Any())
        {
            var rolePolicies = await _rolePermissionRepository.GetPoliciesByRolesAsync(
                userRoleNames,
                cancellationToken);
            policies.AddRange(rolePolicies);
        }

        var groups = await _userGroupRepository.GetGroupsByUserAsync(user, cancellationToken);
        if (groups.Any())
        {
            var groupPolicies = await _groupPolicyRepository.GetPoliciesByGroupAsync(groups, cancellationToken);
            policies.AddRange(groupPolicies);
        }

        var userPolicies = await _userPolicyRepository.GetPoliciesByUserAsync(user, cancellationToken);
        policies.AddRange(userPolicies);

        policies = policies
            .DistinctBy(policy => policy.Id)
            .ToList();

        var permissions = await _policyPermissionRepository.GetPermissionsByPoliciesAsync(policies, cancellationToken);
        var isAllowed = HasRequiredPermissions(permissions, request.RequiredPermissions);
        
        var currentUser = new CurrentUserModel
        {
            User = user,
            Roles = userRoleNames,
            Policies = policies,
            Permissions = permissions,
            Groups = groups,
            IsAllowed = isAllowed
        };
        
        return currentUser;
    }

    private static bool HasRequiredPermissions(
        IEnumerable<PermissionsEntity> userPermissions,
        IReadOnlyList<string>? requiredPermissions)
    {
        if (requiredPermissions is null || requiredPermissions.Count == 0)
            return true;

        var permissionNames = userPermissions
            .Select(ToPermissionName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return requiredPermissions.All(permissionNames.Contains);
    }

    private static string ToPermissionName(PermissionsEntity permission)
        => $"{permission.Resource}:{permission.Action}";
}
