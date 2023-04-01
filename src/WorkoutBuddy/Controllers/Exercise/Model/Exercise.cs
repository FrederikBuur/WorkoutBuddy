using System.Collections.Generic;

namespace WorkoutBuddy.Controllers.Exercise.Model;

public record Exercise(
    Guid id, 
    Guid creatorId, 
    string name, 
    string? description, 
    string? imageUrl,
    MuscleGroupType primaryMuscleGroup,
    ICollection<MuscleGroupType> secondaryMuscleGroup
);
