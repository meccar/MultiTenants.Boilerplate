using Finbuckle.MultiTenant.Abstractions;
using MultiTenants.Boilerplate.Application.Abstractions;

namespace MultiTenants.Boilerplate.Infrastructure.Helpers;

/// <summary>
/// Helper that provides the current tenant (id, name, identifier) from the request context.
/// Wraps Finbuckle's IMultiTenantContextAccessor so Application layer does not reference Finbuckle.
/// </summary>
public sealed class TenantHelper : ITenantProvider
{
    private readonly IMultiTenantContextAccessor<TenantInfo> _accessor;

    public TenantHelper(IMultiTenantContextAccessor<TenantInfo> accessor)
    {
        _accessor = accessor;
    }

    /// <inheritdoc />
    public CurrentTenant? GetCurrentTenant()
    {
        var info = _accessor.MultiTenantContext?.TenantInfo;
        if (info == null)
            return null;

        return new CurrentTenant
        {
            Id = info.Id ?? string.Empty,
            Name = info.Name ?? string.Empty,
            Identifier = info.Identifier ?? string.Empty
        };
    }

    /// <inheritdoc />
    public string? GetCurrentTenantId() => GetCurrentTenant()?.Id;
}
