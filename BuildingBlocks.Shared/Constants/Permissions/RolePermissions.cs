using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class RolePermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.Role, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.Role, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.Role, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.Role, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.Role, Actions.Delete);
    public static readonly string AssignToUser = PermissionBuilder.Build(
        Resources.Role, "AssignToUser");
    public static readonly string RevokeFromUser = PermissionBuilder.Build(
        Resources.Role, "RevokeFromUser");
    public static readonly string AssignPermission = PermissionBuilder.Build(
        Resources.Role, "AssignPermission");
    public static readonly string RevokePermission = PermissionBuilder.Build(
        Resources.Role, "RevokePermission");
    public static readonly string Clone = PermissionBuilder.Build(
        Resources.Role, "Clone");
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.Role, Actions.Manage);
}