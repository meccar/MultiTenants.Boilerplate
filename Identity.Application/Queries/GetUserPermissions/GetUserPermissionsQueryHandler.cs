using Identity.Application.Mapper;
using BuildingBlocks.Shared.Helpers;
using Identity.Domain.Interfaces;
using Identity.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Entities;

namespace Identity.Application.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, CurrentUserModel>
{
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly JwtToken _jwtToken;
    private readonly UserManager<UsersEntity> _userManager;
    private readonly RoleManager<RolesEntity> _roleManager;
    private readonly IPermissionsRepository _permissionsRepository;
    
    public GetUserPermissionsQueryHandler(
        ILogger<GetUserPermissionsQueryHandler> logger,
        JwtToken jwtToken,
        UserManager<UsersEntity> userManager,
        RoleManager<RolesEntity> roleManager,
        IPermissionsRepository permissionsRepository
    )
    {
        _logger = logger;
        _jwtToken = jwtToken;
        _userManager = userManager;
        _roleManager = roleManager;
        _permissionsRepository = permissionsRepository;
    }

    public async Task<CurrentUserModel> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var validateTokenResult = _jwtToken.ValidateToken(request.Token);
        if (!validateTokenResult.IsValid)
            throw new UnauthorizedAccessException();

        var user = await _userManager.FindByEmailAsync(validateTokenResult.Username!);
        if (user == null)
            throw new UnauthorizedAccessException();

        var userRoleNames = await _userManager.GetRolesAsync(user);

        var permissions = await _permissionsRepository
            .GetPermissionsByRolesAsync(userRoleNames, cancellationToken);

        var policies = await _permissionsRepository
            .GetPoliciesByRolesAsync(userRoleNames, cancellationToken);

        _logger.LogInformation(
            "Resolved {RoleCount} roles, {PermissionCount} permissions, and {PolicyCount} policies for user {UserId}.",
            userRoleNames.Count,
            permissions.Count,
            policies.Count,
            user.Id);

        return new CurrentUserModel
        {
            Users = user.ToUsersModel(),
            // Roles = allRoles.ToRolesModel(),
            Roles = userRoleNames,
            Permissions = permissions.ToPermissionsModel(),
            Policies = policies.ToPoliciesModel()
        };
    }
}
