using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class TenantPermissions
{
    public static readonly string View = PermissionBuilder.Build(
        Resources.Tenant, Actions.View);
    public static readonly string List = PermissionBuilder.Build(
        Resources.Tenant, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.Tenant, Actions.Create);
    public static readonly string Update = PermissionBuilder.Build(
        Resources.Tenant, Actions.Update);
    public static readonly string Delete = PermissionBuilder.Build(
        Resources.Tenant, Actions.Delete);
    public static readonly string Activate = PermissionBuilder.Build(
        Resources.Tenant, Actions.Activate);
    public static readonly string Deactivate = PermissionBuilder.Build(
        Resources.Tenant, Actions.Deactivate);
    public static readonly string Transfer = PermissionBuilder.Build(
        Resources.Tenant, Actions.Transfer);
    public static readonly string Purge = PermissionBuilder.Build(
        Resources.Tenant, Actions.Purge);
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.Tenant, Actions.Manage);
}