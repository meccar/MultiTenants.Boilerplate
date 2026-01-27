using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Domain.Entities;

public abstract class BaseEntity
{
    public string Id { get; set; } = UlidGenerator.NewUlid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string TenantId { get; set; } = string.Empty;
}
