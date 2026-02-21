namespace MultiTenants.Boilerplate.Application.Abstractions;

/// <summary>
/// Provides the current tenant (id, name, identifier) for the request context.
/// Implemented in Infrastructure as a helper that wraps Finbuckle's IMultiTenantContextAccessor.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Returns the current tenant entity (id, name, identifier), or null if no tenant context is set.
    /// </summary>
    CurrentTenant? GetCurrentTenant();

    /// <summary>
    /// Returns the current tenant id, or null if no tenant context is set.
    /// </summary>
    string? GetCurrentTenantId() => GetCurrentTenant()?.Id;
}
