namespace Identity.Domain.Entities;

public class RolePolicyEntity
{
    public Guid RoleId { get; set; }
    public Guid PolicyId { get; set; }
    public RolesEntity Roles { get; set; } = null!;
    public PoliciesEntity Policies { get; set; } = null!;
}
