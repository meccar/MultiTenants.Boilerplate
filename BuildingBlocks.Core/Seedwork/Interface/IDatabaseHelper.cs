using BuildingBlocks.Core.Seedwork.Context;

namespace BuildingBlocks.Core.Seedwork.Interface;

public interface IDatabaseHelper
{
    TransactionContext CreateTranctionContext();
}