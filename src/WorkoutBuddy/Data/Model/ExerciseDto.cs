using WorkoutBuddy.Controllers.Exercise.Model;

namespace WorkoutBuddy.Data.Model;

public class ExerciseDto
{
    public Guid? Id { get; set; }

    public Guid? CreatorId { get; set; }

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public MuscleGroupType PrimaryMuscleGroup { get; set; }

    public ICollection<MuscleGroupType> SecondaryMuscleGroups { get; set; } = new List<MuscleGroupType>();

    public override bool Equals(object? obj)
    {
        var other = obj as ExerciseDto;
        if (other is null) return false;

        return this.Id == other.Id &&
        this.CreatorId == other.CreatorId &&
        this.Name == other.Name &&
        this.Description == other.Description &&
        this.ImageUrl == other.ImageUrl &&
        this.PrimaryMuscleGroup == other.PrimaryMuscleGroup &&
        this.SecondaryMuscleGroups.SequenceEqual(other.SecondaryMuscleGroups);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
