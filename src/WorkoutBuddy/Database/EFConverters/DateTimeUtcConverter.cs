using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WorkoutBuddy.EFConverters;

public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    private static Expression<Func<DateTime, DateTime>> convertForward = v => v;
    private static Expression<Func<DateTime, DateTime>> convertBackwards = v => DateTime.SpecifyKind(v, DateTimeKind.Utc);

    public DateTimeUtcConverter() : base(convertForward, convertBackwards, null)
    { }
}