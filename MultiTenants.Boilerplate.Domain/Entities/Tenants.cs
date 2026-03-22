using MultiTenants.Boilerplate.Domain.Seedwork.Entity;

namespace MultiTenants.Boilerplate.Domain.Entities;
public class Tenants : EntityBase
{
    public string Name { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public string Status { get; set; } = null!;
}
