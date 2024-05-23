using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class WorkoutLog : IEntityBase
{
    public DateTime CompletedAt { get; set; }

    // navigation property
    public Guid WorkoutId { get; set; }
    public Workout? Workout { get; set; }
    public ICollection<ExerciseLog>? ExerciseLogs { get; set; }

    // EF Core needs empty constructor
    public WorkoutLog() { }

    public WorkoutLog(Guid? id, DateTime completedAt)
    {
        Id = id ?? Guid.NewGuid();
        CompletedAt = completedAt;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not WorkoutLog other) return false;

        return Id == other.Id
        && CompletedAt == other.CompletedAt;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(CompletedAt);
        hash.Add(ExerciseLogs);
        return hash.ToHashCode();
    }
}
