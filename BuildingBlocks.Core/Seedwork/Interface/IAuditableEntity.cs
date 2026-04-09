namespace BuildingBlocks.Core.Seedwork.Interface;
public interface IAuditableEntity 
    : IDateTracking, ISoftDeletable, IUserTracking;
