using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.Exercise.Model;

public static class ExerciseExtensions
{
    public static ExerciseDto ToExerciseDto(this Data.Model.Exercise e) => new ExerciseDto(
        e.Id,
        e.Owner,
        e.CreatorId,
        e.Name,
        e?.Description,
        e?.ImageUrl,
        e!.IsPublic,
        e.PrimaryMuscleGroup,
        e.SecondaryMuscleGroups
    );

    public static Data.Model.Exercise ToExercise(this ExerciseDto e) => new Data.Model.Exercise
    {
        Id = e.id,
        Owner = e.owner,
        CreatorId = e.creatorId,
        Name = e.name,
        Description = e.description,
        ImageUrl = e.imageUrl,
        IsPublic = e.isPublic,
        PrimaryMuscleGroup = e.primaryMuscleGroup,
        SecondaryMuscleGroups = e.secondaryMuscleGroup
    };

    public static bool ApplyExerciseFilter(this ExerciseFilter filter, Data.Model.Exercise e, Guid? profileId) =>
    filter switch
    {
        ExerciseFilter.PUBLIC => e.IsPublic,
        ExerciseFilter.PRIVATE => e.Owner == profileId,
        ExerciseFilter.ALL => e.IsPublic || e.Owner == profileId,
        _ => false
    };
}
