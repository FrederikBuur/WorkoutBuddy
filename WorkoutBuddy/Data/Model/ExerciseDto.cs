
namespace workouts;

public class ExerciseDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CreatorId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public MuscleGroupType PrimaryMuscleGroup { get; set; }

    public ICollection<MuscleGroupType> SecondaryMuscleGroups { get; set; } = new List<MuscleGroupType>();
}
