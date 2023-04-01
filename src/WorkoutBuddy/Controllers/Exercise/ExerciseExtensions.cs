using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.Exercise.Model;

public static class ExerciseExtensions
{
    public static Exercise ToExercise(this ExerciseDto e) => new Exercise(
        e.Id,
        e.CreatorId,
        e.Name,
        e?.Description,
        e?.ImageUrl,
        e.PrimaryMuscleGroup, 
        e.SecondaryMuscleGroups
    );

    public static ExerciseDto ToExerciseDto(this Exercise e) => new ExerciseDto
    {
        Id = e.id,
        CreatorId = e.creatorId, 
        Name = e.name,
        Description = e.description,
        ImageUrl = e.imageUrl,
        PrimaryMuscleGroup = e.primaryMuscleGroup, 
        SecondaryMuscleGroups = e.secondaryMuscleGroup
    };
}
