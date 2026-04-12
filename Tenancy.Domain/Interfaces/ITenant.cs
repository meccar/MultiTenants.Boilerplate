namespace Tenancy.Domain.Interfaces
{
    public interface ITenant
    {
        string? TenantName { get; }
        bool IsMultiTenant { get; }
    }
}
