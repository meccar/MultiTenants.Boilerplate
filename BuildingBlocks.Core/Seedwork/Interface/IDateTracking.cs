namespace BuildingBlocks.Core.Seedwork.Interface;
public interface IDateTracking
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
