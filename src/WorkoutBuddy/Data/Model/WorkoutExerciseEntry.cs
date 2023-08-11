
namespace WorkoutBuddy.Data.Model;

public class WorkoutExerciseEntry : IEntityBase
{
    public int Order { get; set; }
    public Guid ExerciseId { get; set; }

    // EF Core needs empty constructor
    public WorkoutExerciseEntry() { }

    public WorkoutExerciseEntry(Guid? id, int order, Guid exerciseId)
    {
        Id = id ?? Guid.NewGuid();
        Order = order;
        ExerciseId = exerciseId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not WorkoutExerciseEntry other) return false;

        return Id == other.Id
            && Order == other.Order
            && ExerciseId == other.ExerciseId;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(Order);
        hash.Add(ExerciseId);
        return hash.ToHashCode();
    }
}