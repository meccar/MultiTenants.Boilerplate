namespace BuildingBlocks.Core.Seedwork.Interface;

public interface ICustomLoggerBase
{
    ILogger CreateInstance();
    ILogger CreateInstance(Type type);
    ILogger CreateInstance(string loggerName);
}