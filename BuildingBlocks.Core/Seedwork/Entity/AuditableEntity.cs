using BuildingBlocks.Core.Seedwork.Interface;

namespace BuildingBlocks.Core.Seedwork.Entity;

public abstract class AuditableEntity : IEntityBase, IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
