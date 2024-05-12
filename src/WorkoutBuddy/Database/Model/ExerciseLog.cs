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

    public ExerciseLog(Guid? id,
        Guid workoutLogId,
        Guid exerciseDetailId)
    {
        Id = id ?? Guid.NewGuid();
        WorkoutLogId = workoutLogId;
        ExerciseDetailId = exerciseDetailId;
    }
}
