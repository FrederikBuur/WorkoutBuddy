
namespace WorkoutBuddy.Controllers.ExerciseModel;

public record ExerciseDto(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsPublic,
    string MuscleGroups
);
