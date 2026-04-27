using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Interfaces;

public interface IPermissionsRepository 
    : IRepositoryBase<PermissionsEntity, Guid>
{
    Task<List<RolesEntity>> GetRolesByUserGroupsAsync(
        Guid userId, CancellationToken cancellationToken);

    Task<List<IdentityRoleClaim<Guid>>> GetRoleClaimsByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken);

    Task<List<PermissionsEntity>> GetPermissionsByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken);

    Task<List<PoliciesEntity>> GetPoliciesByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken);
}
