using BuildingBlocks.Core.Seedwork.Interface;

namespace BuildingBlocks.Core.Seedwork.Entity;

public abstract class AuditableEntity : IEntityBase, IAuditableEntity
{
    public Guid Id { get; set; }
    public long CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public long? DeletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
