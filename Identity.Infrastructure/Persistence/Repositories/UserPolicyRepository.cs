using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class UserPolicyRepository
    : RepositoryBase<UserPolicyEntity, Guid>, IUserPolicyRepository
{
    public UserPolicyRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<PoliciesEntity>> GetPoliciesByUserAsync(
        UsersEntity user, CancellationToken cancellationToken)
    {
        return await _context.UserPolicies
            .Join(_context.Users,
                x => x.UserId,
                u => u.Id,
                (up, p) => new { up.PolicyId, p.Id })
            .Where(x => x.Id == user.Id)
            .Join(_context.Policies,
                x => x.PolicyId,
                p => p.Id,
                (_, p) => p)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}