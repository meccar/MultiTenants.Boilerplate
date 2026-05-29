using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class SessionPermissions
{
    public static readonly string List = PermissionBuilder.Build(
        Resources.Session, Actions.List);
    public static readonly string View = PermissionBuilder.Build(
        Resources.Session, Actions.View);
    public static readonly string Revoke = PermissionBuilder.Build(
        Resources.Session, Actions.Revoke);
    public static readonly string RevokeAll = PermissionBuilder.Build(
        Resources.Session, "RevokeAll");
    public static readonly string Impersonate = PermissionBuilder.Build(
        Resources.Session, Actions.Impersonate);
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.Session, Actions.Manage);
}