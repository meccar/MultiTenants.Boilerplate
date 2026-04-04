namespace BuildingBlocks.Domain.Seedwork.Interface;
public interface IUserTracking
{
    Guid CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}
