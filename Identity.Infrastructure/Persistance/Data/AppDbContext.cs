using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistance.Data;

public class AppDbContext
    : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<PermissionsEntity> Permissions => Set<PermissionsEntity>();
    public DbSet<PoliciesEntity> Policies => Set<PoliciesEntity>();
    public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();
    
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");
        builder.Entity<PoliciesEntity>()
            .Property(policy => policy.Conditions)
            .HasColumnType("jsonb");
    }
}
