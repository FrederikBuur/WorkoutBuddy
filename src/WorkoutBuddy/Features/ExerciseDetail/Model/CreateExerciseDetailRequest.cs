namespace WorkoutBuddy.Features;

public record CreateExerciseDetailRequest(
    Guid owner,
    Guid creatorId,
    string name,
    string? description,
    string? imageUrl,
    bool isPublic,
    string muscleGroup
);
