namespace WorkoutBuddy.Data.Model;

public class Workout
{
    public Guid Id { get; set; }
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Desciption { get; set; }
    public DateTime LastPerformed { get; set; }
    public IEnumerable<Exercise> Exercises { get; set; } = new List<Exercise>();
}