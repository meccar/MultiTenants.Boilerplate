using Identity.Domain.Entities;

namespace Identity.Domain.Model;

public class CurrentUserModel
{
    // public UsersModel Users { get; init; } = new();
    public UsersEntity User { get; init; } = new();
    // public RolesModel Roles { get; init; } = new();
    public IList<string> Roles { get; set; } = [];
    // public PermissionsModel Permissions { get; init; } = new();
    public List<PermissionsEntity> Permissions { get; init; } = new();
    // public PoliciesModel Policies { get; init; } = new();
    public List<PoliciesEntity> Policies { get; init; } = new();
    public bool IsAllowed { get; init; } = true;
    // public IReadOnlyList<string> MissingPermissions { get; init; } = [];
}
