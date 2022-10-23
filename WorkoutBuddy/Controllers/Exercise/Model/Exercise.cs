using System.Collections.Generic;

namespace workouts;

public record Exercise(
    Guid id, 
    Guid creatorId, 
    string name, 
    string description, 
    string? imageUrl,
    IEnumerable<MuscleGroup> muscleGroups
);
