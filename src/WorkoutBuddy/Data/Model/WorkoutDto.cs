namespace WorkoutBuddy.Data.Model;

public class WorkoutDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; }
    public string? Desciption { get; set; }
    public DateTime LastPerformed { get; set; }
    public IEnumerable<Guid> ExerciseIds { get; set; } = new List<Guid>();
}