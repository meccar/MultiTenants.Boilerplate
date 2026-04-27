namespace Identity.Domain.Entities;

public class UserGroupEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public GroupsEntity Groups { get; set; } = null!;
    public UsersEntity Users { get; set; } = null!;
}
