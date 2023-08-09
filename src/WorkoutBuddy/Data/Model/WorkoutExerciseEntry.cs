using System.Text.Json.Serialization;

namespace WorkoutBuddy.Data.Model;

public class WorkoutExerciseEntry : IEntityBase
{
    public int Order { get; set; }
    public Guid ExerciseId { get; set; }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}