using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistance.Data;

public class AppDbContext
    : IdentityDbContext<UsersEntity, RolesEntity, Guid>
{
    private const string SchemaName = "identity";
    private const string TablePrefix = "idn_";

    public DbSet<GroupsEntity> Groups => Set<GroupsEntity>();
    public DbSet<PermissionsEntity> Permissions => Set<PermissionsEntity>();
    public DbSet<PoliciesEntity> Policies => Set<PoliciesEntity>();
    public DbSet<GroupRoleEntity> GroupRoles => Set<GroupRoleEntity>();
    public DbSet<PolicyPermissionEntity> PolicyPermissions => Set<PolicyPermissionEntity>();
    public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();
    public DbSet<RolePolicyEntity> RolePolicies => Set<RolePolicyEntity>();
    public DbSet<UserGroupEntity> UserGroups => Set<UserGroupEntity>();
    
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(SchemaName);

        builder.Entity<UsersEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}users");
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.IsDeleted).HasDefaultValue(false);
        });

        builder.Entity<RolesEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}roles");
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.IsDeleted).HasDefaultValue(false);
        });

        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable($"{TablePrefix}user_roles");
        builder.Entity<IdentityUserClaim<Guid>>()
            .ToTable($"{TablePrefix}user_claims");
        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable($"{TablePrefix}user_logins");
        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable($"{TablePrefix}role_claims");
        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable($"{TablePrefix}user_tokens");

        builder.Entity<GroupsEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}groups");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        builder.Entity<PermissionsEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}permissions");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Resource).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Action).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        builder.Entity<PoliciesEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}policies");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Effect).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Conditions).HasColumnType("jsonb");
        });

        builder.Entity<GroupRoleEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}group_roles");
            entity.HasKey(x => new { x.GroupId, x.RoleId });
            entity.HasOne(x => x.Groups)
                .WithMany()
                .HasForeignKey(x => x.GroupId);
            entity.HasOne(x => x.Roles)
                .WithMany()
                .HasForeignKey(x => x.RoleId);
        });

        builder.Entity<UserGroupEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}user_groups");
            entity.HasKey(x => new { x.UserId, x.GroupId });
            entity.HasOne(x => x.Users)
                .WithMany()
                .HasForeignKey(x => x.UserId);
            entity.HasOne(x => x.Groups)
                .WithMany()
                .HasForeignKey(x => x.GroupId);
        });

        builder.Entity<RolePermissionEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}role_permissions");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.HasOne(x => x.Roles)
                .WithMany()
                .HasForeignKey(x => x.RoleId);
            entity.HasOne(x => x.Permissions)
                .WithMany()
                .HasForeignKey(x => x.PermissionId);
        });

        builder.Entity<RolePolicyEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}role_policies");
            entity.HasKey(x => new { x.RoleId, x.PolicyId });
            entity.HasOne(x => x.Roles)
                .WithMany()
                .HasForeignKey(x => x.RoleId);
            entity.HasOne(x => x.Policies)
                .WithMany()
                .HasForeignKey(x => x.PolicyId);
        });

        builder.Entity<PolicyPermissionEntity>(entity =>
        {
            entity.ToTable($"{TablePrefix}policy_permissions");
            entity.HasKey(x => new { x.PolicyId, x.PermissionId });
            entity.HasOne(x => x.Policies)
                .WithMany()
                .HasForeignKey(x => x.PolicyId);
            entity.HasOne(x => x.Permissions)
                .WithMany()
                .HasForeignKey(x => x.PermissionId);
        });
    }
}
