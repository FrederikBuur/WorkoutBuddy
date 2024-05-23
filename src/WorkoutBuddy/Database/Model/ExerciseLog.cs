using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class ExerciseLog : IEntityBase
{
    // navigation properties
    public Guid WorkoutLogId { get; set; }
    public WorkoutLog? WorkoutLog { get; set; }
    public Guid ExerciseDetailId { get; set; }
    public ExerciseDetail? ExerciseDetail { get; set; }
    public ICollection<ExerciseSet>? ExerciseSets { get; set; }

    // EF Core needs empty constructor
    public ExerciseLog() { }

    public ExerciseLog(Guid? id)
    {
        Id = id ?? Guid.NewGuid();
    }

    public ExerciseLog(Guid? id,
        Guid workoutLogId,
        Guid exerciseDetailId)
    {
        Id = id ?? Guid.NewGuid();
        WorkoutLogId = workoutLogId;
        ExerciseDetailId = exerciseDetailId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not ExerciseLog other) return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(ExerciseSets);
        return hash.ToHashCode();
    }
}
