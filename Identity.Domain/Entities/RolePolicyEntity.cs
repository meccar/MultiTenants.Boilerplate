namespace Identity.Domain.Entities;

public class RolePolicyEntity
{
    public string RoleId { get; set; }
    public int PolicyId { get; set; }
    public AppRole Roles { get; set; }
    public PoliciesEntity Policies { get; set; }
}