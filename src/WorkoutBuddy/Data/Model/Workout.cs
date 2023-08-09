
namespace WorkoutBuddy.Data.Model;

public class Workout : IEntityBase
{
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime LastPerformed { get; set; }
    public IList<WorkoutExerciseEntry> ExerciseEntries { get; set; } = new List<WorkoutExerciseEntry>();
}