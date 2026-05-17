namespace BuildingBlocks.Core.Seedwork.Interface;
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    long? DeletedAt { get; set; }
}
