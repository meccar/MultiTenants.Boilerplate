namespace BuildingBlocks.Shared.Utilities;

public abstract class ScopeRegister
{
    protected const string DefaultStackName = "scope";
    public virtual Type Type { get; protected set; }
    public abstract IEnumerable<IDisposable> AddToScope(object state);
}