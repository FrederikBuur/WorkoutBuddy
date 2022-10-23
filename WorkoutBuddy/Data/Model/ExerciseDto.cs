
namespace workouts;

public class ExerciseDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CreaterId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public IEnumerable<MuscleGroupDto> MuscleGroups { get; set; } = new List<MuscleGroupDto>();
}
