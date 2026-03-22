using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MultiTenants.Boilerplate.Infrastructure.Persistance.Data;

/// <summary>
/// EF Core Identity database context with tenant-aware global query filters.
/// All UserManager/RoleManager queries are automatically scoped by current tenant.
/// Provider (SQL Server, PostgreSQL, etc.) is registered in DI; this class is provider-agnostic.
/// </summary>
public class AppDbContext 
    : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Sentinel used when no tenant context is set — ensures no rows match the query filter.
    /// </summary>

    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
