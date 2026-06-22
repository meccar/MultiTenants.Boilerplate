using BuildingBlocks.Core.Seedwork.Interface;
using Stockroom.Domain.Entities;

namespace Stockroom.Domain.Interfaces;

public interface IStoreRepository
    : IRepositoryBase<StoreEntity, Guid>
{
}