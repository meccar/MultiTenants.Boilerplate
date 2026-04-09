namespace BuildingBlocks.Core.Seedwork.ValueObject;
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(0, HashCode.Combine);
}
