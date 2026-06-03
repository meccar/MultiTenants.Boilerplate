using Identity.Domain.Entities;

namespace Identity.Domain.Model;

public class CurrentUserModel
{
    public UsersEntity User { get; init; } = new();
    public IList<string> Roles { get; set; } = [];
    public List<PoliciesEntity> Policies { get; init; } = new();
    public List<PermissionsEntity> Permissions { get; init; } = new();
    public List<GroupsEntity> Groups { get; init; } = new();
    public bool IsAllowed { get; set; } = true;
}
