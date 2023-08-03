namespace WorkoutBuddy.Data.Model;

public class Workout
{
    public Guid Id { get; set; }
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime LastPerformed { get; set; }
    public IEnumerable<Guid> ExerciseIds { get; set; } = new List<Guid>();

    public Workout(
        Guid id,
        Guid owner,
        Guid creatorId,
        string name,
        string? description,
        bool isPublic,
        DateTime lastPerformed,
        IEnumerable<Guid> exerciseIds)
    {
        Id = id;
        Owner = owner;
        CreatorId = creatorId;
        Name = name;
        Description = description;
        IsPublic = isPublic;
        LastPerformed = lastPerformed;
        ExerciseIds = exerciseIds;
    }
}