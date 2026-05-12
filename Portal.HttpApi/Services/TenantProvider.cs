using BuildingBlocks.Core.Abstractions;
using Tenancy.Domain.Interfaces;

namespace Host.Services;

public sealed class TenantProvider : ITenantProvider
{
    private readonly ITenant _tenant;

    public TenantProvider(ITenant tenant)
    {
        _tenant = tenant;
    }

    public CurrentTenant? GetCurrentTenant()
    {
        if (string.IsNullOrWhiteSpace(_tenant.TenantId))
            return null;

        return new CurrentTenant
        {
            Id = _tenant.TenantId,
            Name = _tenant.TenantId,
            Identifier = _tenant.TenantId
        };
    }
}
