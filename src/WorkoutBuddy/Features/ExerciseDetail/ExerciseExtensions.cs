using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

public static class ExerciseDetailExtensions
{
    public static ExerciseDetailDto ToExerciseDetailDto(this ExerciseDetail e) => new(
        e.Id,
        e.Owner,
        e.CreatorId,
        e.Name,
        e?.Description,
        e?.ImageUrl,
        e!.IsPublic,
        e.MuscleGroups
    );

    public static ExerciseDetail ToExerciseDetail(this ExerciseDetailDto e) => new(
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
