using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IUserPolicyRepository
    : IRepositoryBase<UserPolicyEntity, Guid>
{
    Task<List<PoliciesEntity>> GetPoliciesByUserAsync(
        UsersEntity user, CancellationToken cancellationToken);
}