
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

    // EF Core needs empty constructor
    public Workout() { }

    public Workout(Guid? id, Guid owner, Guid creatorId, string name, string? description, bool isPublic, DateTime lastPerformed, IList<WorkoutExerciseEntry> exerciseEntries)
    {
        Id = id ?? Guid.NewGuid();
        Owner = owner;
        CreatorId = creatorId;
        Name = name;
        Description = description;
        IsPublic = isPublic;
        LastPerformed = lastPerformed;
        ExerciseEntries = exerciseEntries;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not Workout other) return false;

        return Id == other.Id
            && Owner == other.Owner
            && CreatorId == other.CreatorId
            && Name == other.Name
            && Description == other.Description
            && IsPublic == other.IsPublic
            && LastPerformed == other.LastPerformed
            && Enumerable.SequenceEqual(ExerciseEntries, other.ExerciseEntries);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(Owner);
        hash.Add(CreatorId);
        hash.Add(Name);
        hash.Add(Description);
        hash.Add(IsPublic);
        hash.Add(LastPerformed);
        hash.Add(ExerciseEntries);
        return hash.ToHashCode();
    }
}