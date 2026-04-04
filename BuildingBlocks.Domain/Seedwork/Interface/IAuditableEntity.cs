namespace BuildingBlocks.Domain.Seedwork.Interface;
public interface IAuditableEntity 
    : IDateTracking, ISoftDeletable, IUserTracking;
