namespace Identity.Domain.Entities;

public class RolePermissionEntity
{
    public string RoleId { get; set; }
    public int PermissionId { get; set; }
    public PermissionsEntity Permissions { get; set; }
    public AppRole Roles { get; set; }
}