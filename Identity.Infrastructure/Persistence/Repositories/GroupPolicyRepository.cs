using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class GroupPolicyRepository
    : RepositoryBase<GroupPolicyEntity, Guid>, IGroupPolicyRepository
{
    public GroupPolicyRepository(AppDbContext context)
        : base(context)
    {
    }
    
    public async Task<List<PoliciesEntity>> GetPoliciesByGroupAsync(
        List<GroupsEntity> groups, CancellationToken cancellationToken)
    {
        var groupIds = groups
            .Select(group => group.Id)
            .ToHashSet();

        return await _context.GroupPolicies
            .Where(groupPolicy => groupIds.Contains(groupPolicy.GroupId))
            .Join(_context.Policies,
                groupPolicy => groupPolicy.PolicyId,
                policy => policy.Id,
                (_, policy) => policy)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
