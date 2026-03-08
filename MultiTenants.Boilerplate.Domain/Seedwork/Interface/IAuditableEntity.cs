namespace MultiTenants.Boilerplate.Domain.Seedwork.Interface;
public interface IAuditableEntity 
    : IDateTracking, ISoftDeletable, IUserTracking;
