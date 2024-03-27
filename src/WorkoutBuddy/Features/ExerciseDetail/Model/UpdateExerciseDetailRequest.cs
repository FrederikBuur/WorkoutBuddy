namespace WorkoutBuddy.Features;

public record UpdateExerciseDetailRequest(
    Guid id,
    Guid owner,
    Guid creatorId,
    string name,
    string? description,
    string? imageUrl,
    bool isPublic,
    string muscleGroups
);