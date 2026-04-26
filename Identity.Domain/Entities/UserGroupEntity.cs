namespace Identity.Domain.Entities;

public class UserGroupEntity
{
    public string UserId { get; set; }
    public int GroupId { get; set; }
    public GroupsEntity Groups { get; set; }
    public AppUser Users { get; set; }
}