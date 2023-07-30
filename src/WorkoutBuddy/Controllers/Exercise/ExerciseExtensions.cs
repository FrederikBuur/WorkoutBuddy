using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.ExerciseModel;

public static class ExerciseExtensions
{
    public static ExerciseDto ToExerciseDto(this Exercise e) => new ExerciseDto(
        e.Id,
        e.Owner,
        e.CreatorId,
        e.Name,
        e?.Description,
        e?.ImageUrl,
        e!.IsPublic,
        e.MuscleGroups
    );

    public static Data.Model.Exercise ToExercise(this ExerciseDto e) => new Data.Model.Exercise
    (
        id: e.id,
        owner: e.owner,
        creatorId: e.creatorId,
        name: e.name,
        description: e.description,
        imageUrl: e.imageUrl,
        isPublic: e.isPublic,
        muscleGroups: e.muscleGroups
    );

    // public static bool ApplyMuscleGroupFilter(this MuscleGroupType? muscleGroupType, Data.Model.Exercise e) =>
    //     muscleGroupType != null && e.MuscleGroups.Contains((MuscleGroupType)muscleGroupType);
}
