using BuildingBlocks.Core.Seedwork.Interface;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

public class RolesEntity 
    : IdentityRole<Guid>, IAuditableEntity
{
    public long CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public long? DeletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}