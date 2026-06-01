using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IGroupPolicyRepository
    : IRepositoryBase<GroupPolicyEntity, Guid>
{
    Task<List<PoliciesEntity>> GetPoliciesByGroupAsync(
        List<GroupsEntity> groups, CancellationToken cancellationToken);
}