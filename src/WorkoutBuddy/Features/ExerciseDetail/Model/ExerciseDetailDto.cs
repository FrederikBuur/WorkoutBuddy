
namespace WorkoutBuddy.Features;

public record ExerciseDetailDto(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsPublic,
    string MuscleGroups
);
