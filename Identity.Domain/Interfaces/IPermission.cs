namespace Identity.Domain.Interfaces;

public interface IPermissionService
{
    Task<(IList<string> Roles, IList<string> Permissions)>
        GetUserRolesAndPermissionsAsync(Guid userId, Guid? tenantId,
            CancellationToken ct = default);
}
