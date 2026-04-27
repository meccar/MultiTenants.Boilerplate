using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IRolePermissionRepository
    : IRepositoryBase<RolePermissionEntity, Guid>
{
    
}