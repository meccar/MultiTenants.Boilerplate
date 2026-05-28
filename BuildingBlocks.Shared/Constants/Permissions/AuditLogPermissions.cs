using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class AuditLogPermissions
{
    public static readonly string List = PermissionBuilder.Build(
        Resources.AuditLog, Actions.List);
    public static readonly string View = PermissionBuilder.Build(
        Resources.AuditLog, Actions.View);
    public static readonly string Read = PermissionBuilder.Build(
        Resources.AuditLog, Actions.Read);
    public static readonly string Export = PermissionBuilder.Build(
        Resources.AuditLog, Actions.Export);
    public static readonly string Purge = PermissionBuilder.Build(
        Resources.AuditLog, Actions.Purge);
}