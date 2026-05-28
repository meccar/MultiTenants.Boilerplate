using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class PolicyPermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.Policy, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.Policy, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.Policy, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.Policy, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.Policy, Actions.Delete);
    public static readonly string Attach = PermissionBuilder.Build(
        Resources.Policy, "Attach");
    public static readonly string Detach = PermissionBuilder.Build(
        Resources.Policy, "Detach");
    public static readonly string Evaluate = PermissionBuilder.Build(
        Resources.Policy, "Evaluate");
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.Policy, Actions.Manage);
}