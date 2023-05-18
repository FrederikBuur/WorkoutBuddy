using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WorkoutBuddy.EFConverters;
public class RecordArrayComparer : ValueComparer<string[]>
{

    private readonly static new Expression<Func<string[]?, string[]?, bool>> EqualsExpression = (a, b) => a == null || b == null ? a == b : a.SequenceEqual(b);
    private readonly static new Expression<Func<string[], int>> HashCodeExpression = a => a.Aggregate(0, (acc, item) => HashCode.Combine(acc, item.GetHashCode()));

    public RecordArrayComparer() : base(EqualsExpression, HashCodeExpression) { }
}