namespace BuildingBlocks.Shared.Utilities;

public class ScopeRegistry
{
    private readonly IDictionary<Type, Func<object, IEnumerable<IDisposable>>> registry = new  Dictionary<Type, Func<object, IEnumerable<IDisposable>>>();

    public ScopeRegistry()
    {
        SetRegister(new ObjectScopeRegister());
        SetRegister(new StringScopeRegister());
        SetRegister(new EnumerableScopeRegister());
        SetRegister(new KeyValuePairScopeRegister());
    }

    public Func<object, IEnumerable<IDisposable>> GetRegister(Type type)
    {
        if (registry.ContainsKey(type))
            return registry[type];

        foreach (KeyValuePair<Type, Func<object, IEnumerable<IDisposable>>> item in registry.Where((KeyValuePair<Type, Func<object, IEnumerable<IDisposable>>> x) => x.Key != typeof(object)))
            if(item.Key.IsAssignableFrom(type))
                return item.Value;
        
        return registry[typeof(object)];
    }

    public ScopeRegistry SetRegister(ScopeRegister property)
    {
        return SetRegister(property.Type, property.AddToScope);
    }

    public ScopeRegistry SetRegister(Type type, Func<object, IEnumerable<IDisposable>> register)
    {
        if (registry.ContainsKey(type))
            registry[type] = register;
        else
            registry.Add(type, register);
        return this;
    }
}