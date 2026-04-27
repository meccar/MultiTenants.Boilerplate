namespace Identity.Domain.Entities;

public class GroupRoleEntity
{
    public Guid RoleId { get; set; }
    public Guid GroupId { get; set; }
    public GroupsEntity Groups { get; set; } = null!;
    public RolesEntity Roles { get; set; } = null!;
}
