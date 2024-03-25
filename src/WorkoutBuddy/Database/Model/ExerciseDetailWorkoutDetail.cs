using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class ExerciseDetailWorkoutDetail : IEntityBase
{
    public Guid WorkoutDetailId { get; set; }
    public Guid ExerciseDetailId { get; set; }
}