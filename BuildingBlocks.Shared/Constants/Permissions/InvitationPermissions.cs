using BuildingBlocks.Shared.Helpers;

namespace BuildingBlocks.Shared.Constants.Permissions;

public class InvitationPermissions
{
    public static readonly string List = PermissionBuilder.Build(
        Resources.Invitation, Actions.List);
    public static readonly string Create = PermissionBuilder.Build(
        Resources.Invitation, Actions.Create);
    public static readonly string Resend = PermissionBuilder.Build(
        Resources.Invitation, "Resend");
    public static readonly string Revoke = PermissionBuilder.Build(
        Resources.Invitation, Actions.Revoke);
    public static readonly string Approve = PermissionBuilder.Build(
        Resources.Invitation, Actions.Approve);
    public static readonly string Manage = PermissionBuilder.Build(
        Resources.Invitation, Actions.Manage);
}