using BuildingBlocks.Core.Seedwork.Interface;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

public class AppUser 
    : IdentityUser, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}