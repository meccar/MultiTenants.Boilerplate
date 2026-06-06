using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class UserAccountPermissions
{
    public static readonly string Zero = PermissionBuilder.Build(
        Resources.UserAccount, Actions.Zero);
    public static readonly string View = PermissionBuilder.Build(
        Resources.UserAccount, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.UserAccount, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.UserAccount, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.UserAccount, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.UserAccount, Actions.Delete);
    public static readonly string AssignRole = PermissionBuilder.Build(
        Resources.UserAccount, Actions.AssignRole);
    public static readonly string ResetPassword = PermissionBuilder.Build(
        Resources.UserAccount, Actions.ResetPassword);
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.UserAccount, Actions.Manage);
}