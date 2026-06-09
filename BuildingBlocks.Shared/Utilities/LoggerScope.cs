namespace BuildingBlocks.Shared.Utilities;

public class LoggerScope : IDisposable
{
    private bool disposedValue;
    private readonly Stack<IDisposable> disposables = new Stack<IDisposable>();
    private readonly ScopeRegistry registry;

    public LoggerScope(object scope, ScopeRegistry registry)
    {
        if (scope == null)
            throw new ArgumentNullException(nameof(scope));
        
        Type type = scope.GetType();
        foreach (IDisposable item in registry.GetRegister(type)(scope))
            disposables.Push(item);
        
        this.registry = registry;
    }

    public void Add(string key, object value)
    {
        KeyValuePair<string, object> keyValuePair = new KeyValuePair<string, object>(key, value);
        foreach (IDisposable item in registry.GetRegister(typeof(KeyValuePair<string, object>))(keyValuePair))
            disposables.Push(item);
    }

    ~LoggerScope()
    {
        Dispose(disposing: false);
    }
    
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
            return;

        if (disposing)
        {
            while (disposables.Count > 0)
                disposables.Pop().Dispose();
            
            disposables.Clear();
        }
        
        disposedValue = true;
    }
}