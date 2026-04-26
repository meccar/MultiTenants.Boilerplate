namespace Identity.Domain.Entities;

public class GroupRoleEntity
{
    public string RoleId { get; set; }
    public int GroupId { get; set; }
    public GroupsEntity Groups { get; set; }
    public AppRole Roles { get; set; }
}