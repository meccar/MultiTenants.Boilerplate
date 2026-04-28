using Identity.Application.Mapper;
using BuildingBlocks.Shared.Helpers;
using Identity.Domain.Interfaces;
using Identity.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, CurrentUserModel>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly JwtToken _jwtToken;
    private readonly UserManager<UsersEntity> _userManager;
    private readonly IPermissionsRepository _permissionsRepository;
    
    public GetUserPermissionsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<GetUserPermissionsQueryHandler> logger,
        JwtToken jwtToken,
        UserManager<UsersEntity> userManager,
        IPermissionsRepository permissionsRepository
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _jwtToken = jwtToken;
        _userManager = userManager;
        _permissionsRepository = permissionsRepository;
    }

    public async Task<CurrentUserModel> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken = default)
    {
        var token = ResolveToken();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException();

        var validateTokenResult = _jwtToken.ValidateToken(token);
        if (!validateTokenResult.IsValid)
            throw new UnauthorizedAccessException();

        var user = await _userManager
            .FindByEmailAsync(validateTokenResult.Username!);
        if (user == null)
            throw new UnauthorizedAccessException();

        var userRoleNames = await _userManager.GetRolesAsync(user);

        var permissions = await _permissionsRepository
            .GetPermissionsByRolesAsync(userRoleNames, cancellationToken);

        var policies = await _permissionsRepository
            .GetPoliciesByRolesAsync(userRoleNames, cancellationToken);

        var currentUser = new CurrentUserModel
        {
            Users = user.ToUsersModel(),
            Roles = userRoleNames,
            Permissions = permissions.ToPermissionsModel(),
            Policies = policies.ToPoliciesModel()
        };

        var missingPermissions = request.RequiredPermissions is { Count: > 0 }
            ? request.RequiredPermissions
                .Where(requiredPermission => !HasAccess(currentUser, requiredPermission))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            : [];

        _logger.LogInformation(
            "Resolved {RoleCount} roles, {PermissionCount} permissions, and {PolicyCount} policies for user {UserName}. Authorization result: {IsAllowed}.",
            userRoleNames.Count,
            permissions.Count,
            policies.Count,
            user.UserName,
            missingPermissions.Count == 0);

        return new CurrentUserModel
        {
            Users = currentUser.Users,
            Roles = currentUser.Roles,
            Permissions = currentUser.Permissions,
            Policies = currentUser.Policies,
            IsAllowed = missingPermissions.Count == 0,
        };
    }

    private string? ResolveToken()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        return authorizationHeader["Bearer ".Length..].Trim();
    }

    private static bool HasAccess(CurrentUserModel currentUser, string requiredPermission)
    {
        var normalizedPermission = requiredPermission.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedPermission))
            return false;

        var permissionMatches = BuildPermissionSet(currentUser.Permissions)
            .Contains(normalizedPermission);

        if (!permissionMatches)
            return false;

        var matchingPolicies = BuildPolicies(currentUser.Policies)
            .Where(policy =>
                policy.Name.Equals(normalizedPermission, StringComparison.OrdinalIgnoreCase) ||
                policy.Name.Equals("*", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingPolicies.Any(policy =>
                policy.Effect.Equals("deny", StringComparison.OrdinalIgnoreCase)))
            return false;

        var allowPolicies = matchingPolicies
            .Where(policy => policy.Effect.Equals("allow", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return allowPolicies.Count == 0 || allowPolicies.Any();
    }

    private static HashSet<string> BuildPermissionSet(PermissionsModel permissions)
    {
        var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in permissions.Names)
        {
            if (!string.IsNullOrWhiteSpace(name))
                values.Add(name.Trim().ToLowerInvariant());
        }

        var count = Math.Min(permissions.Resources.Count, permissions.Actions.Count);
        for (var index = 0; index < count; index++)
        {
            var resource = permissions.Resources[index];
            var action = permissions.Actions[index];

            if (string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(action))
                continue;

            values.Add($"{resource.Trim().ToLowerInvariant()}:{action.Trim().ToLowerInvariant()}");
        }

        return values;
    }

    private static IEnumerable<(string Name, string Effect, string Conditions)> BuildPolicies(
        PoliciesModel policies)
    {
        var count = new[]
        {
            policies.Names.Count,
            policies.Effects.Count,
            policies.Conditions.Count
        }.Min();

        for (var index = 0; index < count; index++)
        {
            yield return (
                policies.Names[index],
                policies.Effects[index],
                policies.Conditions[index]);
        }
    }
}
