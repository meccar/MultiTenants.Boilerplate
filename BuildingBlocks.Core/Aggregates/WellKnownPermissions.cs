using BuildingBlocks.Shared.Constants.Permissions;

namespace BuildingBlocks.Core.Aggregates;

public static class WellKnownPermissions
{
    public static readonly IReadOnlyList<string> SuperAdmin = new[]
    {
        UserAccountPermissions.Manage,
        RolePermissions.Manage,
        PolicyPermissions.Manage,
        TenantPermissions.Manage,
        AuditLogPermissions.Export,
        AuditLogPermissions.Purge,
        ServiceAccountPermissions.Manage,
        SsoProviderPermissions.Manage,
    };

    public static readonly IReadOnlyList<string> TenantAdmin = new[]
    {
        UserAccountPermissions.Manage,
        RolePermissions.Manage,
        PolicyPermissions.Manage,
        InvitationPermissions.Manage,
        SessionPermissions.RevokeAll,
        AuditLogPermissions.List,
        AuditLogPermissions.Export,
        ApiKeyPermissions.Manage,
        SsoProviderPermissions.Manage,
    };
}