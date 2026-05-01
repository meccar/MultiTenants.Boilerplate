using Identity.Domain.Entities;
using Identity.Domain.Model;
using System.Text.Json;

namespace Identity.Application.Mapper;

public static class CurrentUserModelMapper
{
    public static UsersModel ToUsersModel(this UsersEntity user)
        => new()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed
        };

    public static RolesModel ToRolesModel(this IEnumerable<RolesEntity> roles)
    {
        var roleList = roles.ToList();

        return new RolesModel
        {
            Ids = roleList.Select(role => role.Id).ToList(),
            Names = roleList
                .Select(role => role.Name ?? string.Empty)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }

    public static PermissionsModel ToPermissionsModel(this IEnumerable<PermissionsEntity> permissions)
    {
        var permissionList = permissions.ToList();

        return new PermissionsModel
        {
            Ids = permissionList.Select(permission => permission.Id).ToList(),
            Names = permissionList.Select(permission => permission.Name).ToList(),
            Resources = permissionList.Select(permission => permission.Resource).ToList(),
            Actions = permissionList.Select(permission => permission.Action).ToList(),
            Descriptions = permissionList.Select(permission => permission.Description).ToList()
        };
    }

    public static PoliciesModel ToPoliciesModel(this IEnumerable<PoliciesEntity> policies)
    {
        var policyList = policies.ToList();

        return new PoliciesModel
        {
            Ids = policyList.Select(policy => policy.Id).ToList(),
            Names = policyList.Select(policy => policy.Name).ToList(),
            Effects = policyList.Select(policy => policy.Effect).ToList(),
            Conditions = policyList
                .Select(policy => policy.Conditions.RootElement.GetRawText())
                .ToList()
        };
    }
}
