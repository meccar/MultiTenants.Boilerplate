namespace Identity.Domain.Entities;

public class PolicyPermissionEntity
{
    public Guid PolicyId { get; set; }
    public Guid PermissionId { get; set; }
    public PermissionsEntity Permissions { get; set; } = null!;
    public PoliciesEntity Policies { get; set; } = null!;
}
