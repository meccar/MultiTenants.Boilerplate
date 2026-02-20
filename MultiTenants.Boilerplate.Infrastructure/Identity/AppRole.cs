using Microsoft.AspNetCore.Identity;

namespace MultiTenants.Boilerplate.Infrastructure.Identity;

/// <summary>
/// Identity role extended only with TenantId for multi-tenant isolation.
/// No reimplementation of Identity — only adds tenant awareness.
/// </summary>
public class AppRole : IdentityRole
{
    /// <summary>
    /// Tenant identifier. All queries are scoped by this value via EF Core global query filter.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}
