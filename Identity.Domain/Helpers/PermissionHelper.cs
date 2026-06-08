using BuildingBlocks.Shared.Constants.Permissions;
using Identity.Domain.Entities;

namespace Identity.Domain.Helpers;

public static class PermissionHelper
{
    public static string ToPermissionName(PermissionsEntity permission)
        => ToPermissionName(permission.Resource, permission.Action);

    public static string ToPermissionName(string resource, string action)
        => $"{resource}:{action}";

    public static bool HasRequiredPermissions(
        IEnumerable<PermissionsEntity> userPermissions,
        IEnumerable<string>? requiredPermissions)
    {
        if (requiredPermissions is null)
            return true;

        var requiredPermissionNames = requiredPermissions.ToList();
        var userPermissionsNames = userPermissions.ToList();
        if (requiredPermissionNames.Count == 0 
            || userPermissionsNames.Count == 0)
            return true;

        foreach (var requiredPermissionName in requiredPermissionNames)
            if (requiredPermissionName == UserAccountPermissions.Zero)
                return true;
        
        foreach (var userPermissionsName in userPermissionsNames)
            if (ToPermissionName(userPermissionsName) == UserAccountPermissions.Manage)
                return true;

        var permissionNames = userPermissionsNames
            .Select(ToPermissionName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return requiredPermissionNames.All(permissionNames.Contains);
    }
}
