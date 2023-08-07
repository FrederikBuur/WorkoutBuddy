using System.Text.Json.Serialization;

namespace WorkoutBuddy.Data.Model;

public class Workout : IEntityBase
{
    public Guid Id { get; set; }
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime LastPerformed { get; set; }
    public IEnumerable<WorkoutExerciseEntry> Exercises { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Workout(
        Guid id,
        Guid owner,
        Guid creatorId,
        string name,
        string? description,
        bool isPublic,
        DateTime lastPerformed)
    {
        Id = id;
        Owner = owner;
        CreatorId = creatorId;
        Name = name;
        Description = description;
        IsPublic = isPublic;
        LastPerformed = lastPerformed;
        Exercises = new List<WorkoutExerciseEntry>(); ;
    }

    [JsonConstructor]
    public Workout(
    Guid id,
    Guid owner,
    Guid creatorId,
    string name,
    string? description,
    bool isPublic,
    DateTime lastPerformed,
    IEnumerable<WorkoutExerciseEntry> exerciseIds
    ) : this(id, owner, creatorId, name, description, isPublic, lastPerformed)
    {
        Exercises = exerciseIds;
    }
}