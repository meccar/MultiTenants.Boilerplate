namespace BuildingBlocks.Core.Seedwork.Interface;

public interface IAsyncDisposable
{
    ValueTask DisposeAsync();
}