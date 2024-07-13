namespace WorkoutBuddy.Features;

public record CreateExerciseDetailRequest(
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsPublic,
    string MuscleGroup
);
