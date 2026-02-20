namespace MultiTenants.Boilerplate.Application.Abstractions;

/// <summary>
/// Provides the current tenant identifier for the request context.
/// Implemented in Infrastructure using Finbuckle's IMultiTenantContextAccessor.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Returns the current tenant id, or null if no tenant context is set.
    /// </summary>
    string? GetCurrentTenantId();
}
