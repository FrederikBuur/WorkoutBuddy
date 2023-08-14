using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.ExerciseModel;

public static class ExerciseExtensions
{
    public static ExerciseDto ToExerciseDto(this Exercise e) => new(
        e.Id,
        e.Owner,
        e.CreatorId,
        e.Name,
        e?.Description,
        e?.ImageUrl,
        e!.IsPublic,
        e.MuscleGroups
    );

    public static Exercise ToExercise(this ExerciseDto e) => new(
        id: e.Id,
        owner: e.Owner,
        creatorId: e.CreatorId,
        name: e.Name,
        description: e.Description,
        imageUrl: e.ImageUrl,
        isPublic: e.IsPublic,
        muscleGroups: e.MuscleGroups
    );
}
