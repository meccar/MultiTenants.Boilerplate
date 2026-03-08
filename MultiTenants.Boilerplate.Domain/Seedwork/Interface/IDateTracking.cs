namespace MultiTenants.Boilerplate.Domain.Seedwork.Interface;
public interface IDateTracking
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
