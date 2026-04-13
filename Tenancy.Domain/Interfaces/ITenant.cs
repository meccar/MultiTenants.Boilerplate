namespace Tenancy.Domain.Interfaces
{
    public interface ITenant
    {
        string? TenantId { get; }
        bool IsMultiTenant { get; }
    }
}
