namespace WorkoutBuddy.Features;

public record WorkoutDetailDto(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    bool IsPublic,
    IEnumerable<ExerciseDetailDto> Exercises
);