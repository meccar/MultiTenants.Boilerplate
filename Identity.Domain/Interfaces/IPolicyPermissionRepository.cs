using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IPolicyPermissionRepository
    : IRepositoryBase<PolicyPermissionEntity, Guid>
{
    Task<List<PermissionsEntity>> GetPermissionsByPoliciesAsync(
        IEnumerable<Guid> policyIds,
        CancellationToken cancellationToken = default);
}