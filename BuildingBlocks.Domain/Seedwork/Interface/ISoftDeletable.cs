namespace BuildingBlocks.Domain.Seedwork.Interface;
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
