using System.Runtime.CompilerServices;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Core.Seedwork.Interface;

public interface ILogger
{
    void Info(string message, [CallerMemberName] string memberName = "",  [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    void Debug(string message, [CallerMemberName] string memberName = "",  [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    void Warn(string message, [CallerMemberName] string memberName = "",  [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    void Error(string message, [CallerMemberName] string memberName = "",  [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    void Info(string message, Dictionary<string, object> fields);
    void Warn(string message, Dictionary<string, object> fields);
    void Error(string message, Dictionary<string, object> fields);
    LoggerScope CreateScope<TState>(TState state);
}