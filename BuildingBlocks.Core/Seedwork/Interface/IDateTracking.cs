namespace BuildingBlocks.Core.Seedwork.Interface;
public interface IDateTracking
{
    long CreatedAt { get; set; }
    long? UpdatedAt { get; set; }
}
