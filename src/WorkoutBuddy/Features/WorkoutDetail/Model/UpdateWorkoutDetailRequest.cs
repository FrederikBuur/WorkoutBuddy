namespace WorkoutBuddy.Features;

public record UpdateWorkoutDetailRequest(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    bool IsPublic
);