namespace Identity.Domain.Entities;

public class PolicyPermissionEntity
{
    public string PolicyId { get; set; }
    public int PermissionId { get; set; }
    public PermissionsEntity Permissions { get; set; }
    public PoliciesEntity Policies { get; set; }
}