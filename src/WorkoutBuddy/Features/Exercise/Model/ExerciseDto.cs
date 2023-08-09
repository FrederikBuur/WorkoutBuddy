
namespace WorkoutBuddy.Controllers.ExerciseModel;

public record ExerciseDto(
    Guid id,
    Guid owner,
    Guid creatorId,
    string name,
    string? description,
    string? imageUrl,
    bool isPublic,
    string muscleGroups
);
