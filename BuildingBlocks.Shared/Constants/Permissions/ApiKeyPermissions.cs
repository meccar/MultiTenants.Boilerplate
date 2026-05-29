using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class ApiKeyPermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.ApiKey, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.ApiKey, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.ApiKey, Actions.Create);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.ApiKey, Actions.Delete);
    public static readonly string Rotate = PermissionBuilder.Build(
        Resources.ApiKey, Actions.Rotate);
    public static readonly string Revoke = PermissionBuilder.Build(
        Resources.ApiKey, Actions.Revoke);
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.ApiKey, Actions.Manage);
}
