using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class ServiceAccountPermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Delete);
    public static readonly string Rotate = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Rotate);
    public static readonly string Lock = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Lock);
    public static readonly string Unlock = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Unlock);
    public static readonly string AssignRole = PermissionBuilder.Build(
        Resources.ServiceAccount, "AssignRole");
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.ServiceAccount, Actions.Manage);
}