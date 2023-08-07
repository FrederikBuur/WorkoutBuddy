namespace WorkoutBuddy.Data.Model;

public class WorkoutExerciseEntry : IEntityBase
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public Guid ExerciseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WorkoutExerciseEntry(Guid id, int order, Guid exerciseId)
    {
        Id = id;
        Order = order;
        ExerciseId = exerciseId;
    }
}