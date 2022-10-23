namespace workouts;

public record MuscleGroup(
    Guid id, 
    MuscleActivation activation, 
    MuscleGroupType type
);
