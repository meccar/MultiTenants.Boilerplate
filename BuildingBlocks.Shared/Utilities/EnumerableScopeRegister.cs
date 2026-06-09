using System.Collections;
using log4net;

namespace BuildingBlocks.Shared.Utilities;

public sealed class EnumerableScopeRegister : ScopeRegister
{
    public EnumerableScopeRegister()
    {
        Type = typeof(IEnumerable);
    }
    public override IEnumerable<IDisposable> AddToScope(object state)
    {
        if (!(state is IEnumerable enumerable))
            yield break;

        foreach (var item in enumerable)
        {
            Type itemType = item.GetType();
            if (itemType.IsAssignableFrom(typeof(KeyValuePair<string, string>)))
            {
                KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)item;
                yield return LogicalThreadContext.Stacks[keyValuePair.Key].Push(keyValuePair.Value);
            }

            if (itemType.IsAssignableFrom(typeof(KeyValuePair<string, object>)))
            {
                KeyValuePair<string, object> keyValuePair = (KeyValuePair<string, object>)item;
                object value = keyValuePair.Value;
                string? message = ((value is DateTime dateTime)
                    ? dateTime.ISOFormat()
                    : ((!(value is DateTimeOffset dateTime2))
                        ? keyValuePair.Value?.ToString()
                        : dateTime2.ISOFormat()));
                yield return LogicalThreadContext.Stacks[keyValuePair.Key].Push(message);
            }
            
            if (itemType.IsAssignableFrom(typeof(object)))
                yield return LogicalThreadContext.Stacks["scope"].Push(state.ToString());
        }
    }
}