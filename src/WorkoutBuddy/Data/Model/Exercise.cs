namespace WorkoutBuddy.Data.Model;

public class Exercise : IEntityBase
{
    public Guid Owner { get; set; }
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }
    public string MuscleGroups { get; set; } = ""; // comma seperated

    // EF Core needs empty constructor
    public Exercise() { }

    public Exercise(Guid? id, Guid owner, Guid creatorId, string name, string? description, string? imageUrl, bool isPublic, string muscleGroups)
    {
        Id = id ?? Guid.NewGuid();
        Owner = owner;
        CreatorId = creatorId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        IsPublic = isPublic;
        MuscleGroups = muscleGroups;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not Exercise other) return false;

        return Id == other.Id
            && Owner == other.Owner
            && CreatorId == other.CreatorId
            && Name == other.Name
            && Description == other.Description
            && ImageUrl == other.ImageUrl
            && IsPublic == other.IsPublic
            && MuscleGroups == other.MuscleGroups;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(Owner);
        hash.Add(CreatorId);
        hash.Add(Name);
        hash.Add(Description);
        hash.Add(ImageUrl);
        hash.Add(IsPublic);
        hash.Add(MuscleGroups);
        return hash.ToHashCode();
    }
}
