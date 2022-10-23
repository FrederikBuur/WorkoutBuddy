namespace workouts;

public class MuscleGroupDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public MuscleActivation Activation { get; set; }

    public MuscleGroupType Type { get; set; }
}
