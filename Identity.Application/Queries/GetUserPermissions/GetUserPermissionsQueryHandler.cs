using BuildingBlocks.Shared.Exceptions;
using Identity.Domain.Interfaces;
using Identity.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Entities;

namespace Identity.Application.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, CurrentUserModel>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserPolicyRepository _userPolicyRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IGroupPolicyRepository _groupPolicyRepository;
    
    public GetUserPermissionsQueryHandler(
        UserManager<UsersEntity> userManager,
        IRolePermissionRepository rolePermissionRepository,
        IUserPolicyRepository userPolicyRepository,
        IUserGroupRepository userGroupRepository,
        IGroupPolicyRepository groupPolicyRepository
    )
    {
        _userManager = userManager;
        _rolePermissionRepository = rolePermissionRepository;
        _userPolicyRepository = userPolicyRepository;
        _userGroupRepository = userGroupRepository;
        _groupPolicyRepository = groupPolicyRepository;
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
        List<GroupsEntity> groups = [];
        
        var userRoleNames = await _userManager.GetRolesAsync(user);
        if (userRoleNames.Any())
            policies = await _rolePermissionRepository.GetPoliciesByRolesAsync(userRoleNames, cancellationToken);
        if (policies.Any())
        {
            groups = await _userGroupRepository .GetGroupsByUserAsync(user, cancellationToken);
            if (groups.Any())
                policies = await _groupPolicyRepository.GetPoliciesByGroupAsync(groups, cancellationToken);
        }
        if (policies.Any())
            policies = await _userPolicyRepository.GetPoliciesByUserAsync(user, cancellationToken);
        
        var currentUser = new CurrentUserModel
        {
            User = user,
            Roles = userRoleNames,
            Policies = policies,
            Groups = groups
        };
        
        return currentUser;
    }
}
