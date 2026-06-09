using log4net;

namespace BuildingBlocks.Shared.Utilities;

public sealed class StringScopeRegister : ScopeRegister
{
    public StringScopeRegister()
    {
        base.Type = typeof(string);
    }
    
    public override IEnumerable<IDisposable> AddToScope(object state)
    {
        if (state is string message)
            yield return LogicalThreadContext.Stacks["scope"].Push(message);
    }
}