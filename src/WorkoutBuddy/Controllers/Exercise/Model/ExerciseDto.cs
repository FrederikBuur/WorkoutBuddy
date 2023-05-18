
namespace WorkoutBuddy.Controllers.Exercise.Model;

public record ExerciseDto(
    Guid? id,
    Guid owner,
    Guid? creatorId,
    string name,
    string? description,
    string? imageUrl,
    bool isPublic,
    MuscleGroupType primaryMuscleGroup,
    ICollection<MuscleGroupType> secondaryMuscleGroup
);
