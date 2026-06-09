using log4net;

namespace BuildingBlocks.Shared.Utilities;

public sealed class KeyValuePairScopeRegister : ScopeRegister
{
    public KeyValuePairScopeRegister()
    {
        Type = typeof(KeyValuePair<string, string>);
    }
    
    public override IEnumerable<IDisposable> AddToScope(object state)
    {
        KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)state;
        yield return LogicalThreadContext.Stacks[keyValuePair.Key].Push(keyValuePair.Value);
    }
}