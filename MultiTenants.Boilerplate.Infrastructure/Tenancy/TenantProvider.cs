using Finbuckle.MultiTenant.Abstractions;
using MultiTenants.Boilerplate.Application.Abstractions;

namespace MultiTenants.Boilerplate.Infrastructure.Tenancy;

/// <summary>
/// Provides the current tenant id from the request context.
/// Wraps Finbuckle's IMultiTenantContextAccessor so Application layer does not reference Finbuckle.
/// </summary>
public sealed class TenantProvider : ITenantProvider
{
    private readonly IMultiTenantContextAccessor<TenantInfo> _accessor;

    public TenantProvider(IMultiTenantContextAccessor<TenantInfo> accessor)
    {
        _accessor = accessor;
    }

    /// <inheritdoc />
    public string? GetCurrentTenantId() =>
        _accessor.MultiTenantContext?.TenantInfo?.Id;
}
