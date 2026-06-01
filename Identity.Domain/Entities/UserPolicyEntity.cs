namespace Identity.Domain.Entities;

public class UserPolicyEntity
{
    public Guid UserId { get; set; }
    public Guid PolicyId { get; set; }
    public UsersEntity Users { get; set; }
    public PoliciesEntity Policies { get; set; }
}