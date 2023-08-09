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

    public override bool Equals(object? obj)
    {
        var other = obj as Exercise;
        if (other is null) return false;

        return this.Id == other.Id &&
        this.CreatorId == other.CreatorId &&
        this.Name == other.Name &&
        this.Description == other.Description &&
        this.ImageUrl == other.ImageUrl &&
        this.IsPublic == other.IsPublic &&
        this.MuscleGroups.SequenceEqual(other.MuscleGroups);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
