namespace Identity.Domain.Entities;

public class RolePermissionEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public PermissionsEntity Permissions { get; set; } = null!;
    public RolesEntity Roles { get; set; } = null!;
}
