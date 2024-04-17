
namespace WorkoutBuddy.Data.Model;

public class WorkoutDetail : IEntityBase
{
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }

    // navigation properties
    public ICollection<ExerciseDetail> Exercises { get; set; } = new List<ExerciseDetail>();

    // EF Core needs empty constructor
    public WorkoutDetail() { }

    public WorkoutDetail(Guid? id, Guid owner, Guid creatorId, string name, string? description, bool isPublic)
    {
        Id = id ?? Guid.NewGuid();
        Owner = owner;
        CreatorId = creatorId;
        Name = name;
        Description = description;
        IsPublic = isPublic;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not WorkoutDetail other) return false;

        return Id == other.Id
            && Owner == other.Owner
            && CreatorId == other.CreatorId
            && Name == other.Name
            && Description == other.Description
            && IsPublic == other.IsPublic;
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
        hash.Add(Exercises);
        return hash.ToHashCode();
    }
}