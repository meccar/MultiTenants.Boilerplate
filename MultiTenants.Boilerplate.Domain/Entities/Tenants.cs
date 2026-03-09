using MultiTenants.Boilerplate.Domain.Seedwork.Entity;

namespace MultiTenants.Boilerplate.Domain.Entities;
public class Tenants : EntityBase
{
    public string Name { get; set; } = null!;
    public string domain { get; set; } = null!;
    public string status { get; set; } = null!;
}
