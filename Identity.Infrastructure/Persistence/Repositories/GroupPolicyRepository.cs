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
        List<PoliciesEntity> policies = [];
        foreach (var group in groups)
            policies =  await _context.GroupPolicies
                .Join(_context.Groups,
                    rp => rp.GroupId,
                    r => r.Id,
                    (rp, r) => new { rp.PolicyId, r.Id })
                .Where(x => x.Id == group.Id)
                .Join(_context.Policies,
                    x => x.PolicyId,
                    p => p.Id,
                    (_, p) => p)
                .Distinct()
                .ToListAsync(cancellationToken);
        return policies;
    }
}