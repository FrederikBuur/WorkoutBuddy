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

    public static Exercise ToExercise(this ExerciseDto e) => new()
    {
        Id = e.id,
        Owner = e.owner,
        CreatorId = e.creatorId,
        Name = e.name,
        Description = e.description,
        ImageUrl = e.imageUrl,
        IsPublic = e.isPublic,
        MuscleGroups = e.muscleGroups
    };
}
