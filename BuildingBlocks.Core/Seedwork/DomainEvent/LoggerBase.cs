using BuildingBlocks.Core.Seedwork.Interface;
using log4net;

namespace BuildingBlocks.Core.Seedwork.DomainEvent;

public class LoggerBase : ILoggerBase
{
    public static ICustomLoggerBase? CustomLoggerBase { get; set; }
    public ILogger GetLogger()
    {
        return Get();
    }

    public ILogger GetLogger(Type type)
    {
        return Get(type);
    }

    public static ILogger Get()
    {
        return CustomLoggerBase.CreateInstance();
    }

    public static ILogger Get(Type type)
    {
        return CustomLoggerBase.CreateInstance(type);
    }
    
    public static ILogger Get(string loggerName)
    {
        return CustomLoggerBase.CreateInstance(loggerName);
    }

    public static ILogger Get<T>()
    {
        return Get(typeof(T));
    }

    public static void Flush(int millisecondsTimeout = 3000)
    {
        LogManager.Flush(millisecondsTimeout);
    }
}