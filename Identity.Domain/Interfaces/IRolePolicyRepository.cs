using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IRolePolicyRepository
    : IRepositoryBase<RolePolicyEntity, Guid>
{
    Task<List<PoliciesEntity>> GetPoliciesByRolesAsync(
        IList<string> roleNames,
        CancellationToken cancellationToken);
}
