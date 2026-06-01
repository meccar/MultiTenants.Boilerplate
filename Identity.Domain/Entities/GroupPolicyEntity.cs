namespace Identity.Domain.Entities;

public class GroupPolicyEntity
{
    public Guid PolicyId { get; set; }
    public Guid GroupId { get; set; }
    public PoliciesEntity Policies { get; set; }
    public GroupsEntity Groups { get; set; }
}