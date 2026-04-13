using Tenancy.Domain.Interfaces;

namespace Tenancy.Domain.Models
{
    public class TenantContext : ITenant
    {
        public string? TenantId { get; set; }
        public bool IsMultiTenant { get; set; }
    }
}
