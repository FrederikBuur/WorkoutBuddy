using Microsoft.EntityFrameworkCore.ChangeTracking;

public class CollectionValueComparer<T> : ValueComparer<IEnumerable<T>>
{
    // https://gregkedzierski.com/essays/enum-collection-serialization-in-dotnet-core-and-entity-framework-core/
    public CollectionValueComparer() : base((c1, c2) => c1.SequenceEqual(c2),
      c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToHashSet())
    {
    }
}
