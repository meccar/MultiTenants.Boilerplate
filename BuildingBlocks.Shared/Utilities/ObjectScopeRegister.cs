using log4net;

namespace BuildingBlocks.Shared.Utilities;

public class ObjectScopeRegister : ScopeRegister
{
    public ObjectScopeRegister()
    {
        base.Type = typeof(object);
    }
    
    public override IEnumerable<IDisposable> AddToScope(object state)
    {
        if (state != null)
            yield return LogicalThreadContext.Stacks["scope"].Push(state.ToString());
    }
}