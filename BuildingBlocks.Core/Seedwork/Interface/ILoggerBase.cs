namespace BuildingBlocks.Core.Seedwork.Interface;

public interface ILoggerBase
{
    ILogger GetLogger();
    ILogger GetLogger(Type type);
}