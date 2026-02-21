using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Infrastructure.Identity;

namespace MultiTenants.Boilerplate.Infrastructure.Data;

/// <summary>
/// EF Core Identity database context with tenant-aware global query filters.
/// All UserManager/RoleManager queries are automatically scoped by current tenant.
/// Provider (SQL Server, PostgreSQL, etc.) is registered in DI; this class is provider-agnostic.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    /// <summary>
    /// Set in constructor from ITenantProvider so query filters can be translated to SQL.
    /// When null/empty, NoTenantSentinel is used so no rows match.
    /// </summary>
    public string CurrentTenantId { get; }

    /// <summary>
    /// Sentinel used when no tenant context is set — ensures no rows match the query filter.
    /// </summary>
    private const string NoTenantSentinel = "__no_tenant__";

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantProvider tenantProvider)
        : base(options)
    {
        CurrentTenantId = tenantProvider.GetCurrentTenantId() ?? NoTenantSentinel;
    }

    /// <summary>
    /// Design-time constructor for EF Core tools (e.g. migrations). Do not use at runtime.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        CurrentTenantId = "__design_time__";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ----- Global query filters: all queries are tenant-scoped (CurrentTenantId set per-request in ctor) -----
        modelBuilder.Entity<AppUser>().HasQueryFilter(u => u.TenantId == CurrentTenantId);
        modelBuilder.Entity<AppRole>().HasQueryFilter(r => r.TenantId == CurrentTenantId);

        // ----- Replace default single-column unique indexes with composite (TenantId + normalized) -----
        RemoveDefaultUniqueIndexesAndAddComposite(modelBuilder);
    }

    private static void RemoveDefaultUniqueIndexesAndAddComposite(ModelBuilder modelBuilder)
    {
        // AppUser: remove default unique indexes on NormalizedUserName and NormalizedEmail
        var userEntity = modelBuilder.Entity<AppUser>();
        var userMetadata = userEntity.Metadata;
        foreach (var index in userMetadata.GetIndexes().ToList())
        {
            if (index.Properties.Count == 1)
            {
                var propName = index.Properties[0].Name;
                if (propName == nameof(IdentityUser.NormalizedUserName) || propName == nameof(IdentityUser.NormalizedEmail))
                    userMetadata.RemoveIndex(index);
            }
        }

        // AppUser: composite unique indexes (per-tenant email and username)
        userEntity.HasIndex(u => new { u.TenantId, u.NormalizedUserName })
            .HasDatabaseName("IX_AspNetUsers_TenantId_NormalizedUserName")
            .IsUnique();
        userEntity.HasIndex(u => new { u.TenantId, u.NormalizedEmail })
            .HasDatabaseName("IX_AspNetUsers_TenantId_NormalizedEmail")
            .IsUnique();

        // AppRole: remove default unique index on NormalizedName
        var roleEntity = modelBuilder.Entity<AppRole>();
        var roleMetadata = roleEntity.Metadata;
        foreach (var index in roleMetadata.GetIndexes().ToList())
        {
            if (index.Properties.Count == 1 && index.Properties[0].Name == nameof(IdentityRole.NormalizedName))
                roleMetadata.RemoveIndex(index);
        }

        // AppRole: composite unique index (per-tenant role name)
        roleEntity.HasIndex(r => new { r.TenantId, r.NormalizedName })
            .HasDatabaseName("IX_AspNetRoles_TenantId_NormalizedName")
            .IsUnique();
    }
}
