using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class SsoProviderPermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Delete);
    public static readonly string Activate = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Activate);
    public static readonly string Deactivate = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Deactivate);
    public static readonly string TestConnection = PermissionBuilder.Build(
        Resources.SsoProvider, "TestConnection");
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.SsoProvider, Actions.Manage);
}