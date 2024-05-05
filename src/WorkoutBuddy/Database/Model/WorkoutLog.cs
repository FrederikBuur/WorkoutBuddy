using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class WorkoutLog : IEntityBase
{
    public DateTime CompletedAt { get; set; }

    // navigation property
    public Guid WorkoutId { get; set; }
    public Workout? Workout { get; set; }
    public ICollection<ExerciseLog> ExerciseLog { get; set; } = new List<ExerciseLog>();

    // EF Core needs empty constructor
    public WorkoutLog() { }

    public WorkoutLog(Guid? id, DateTime completedAt)
    {
        Id = id ?? Guid.NewGuid();
        CompletedAt = completedAt;
    }
}
