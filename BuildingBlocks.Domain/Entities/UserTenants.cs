namespace BuildingBlocks.Domain.Entities
{
    public class UserTenants
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
    }
}
