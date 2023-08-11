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
        id: e.id,
        owner: e.owner,
        creatorId: e.creatorId,
        name: e.name,
        description: e.description,
        imageUrl: e.imageUrl,
        isPublic: e.isPublic,
        muscleGroups: e.muscleGroups
    );
}
