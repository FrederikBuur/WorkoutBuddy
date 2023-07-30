
using WorkoutBuddy.Controllers.ExerciseModel;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.Workout.Model;

public static class WorkoutExtensions
{
    public static WorkoutDto MapToWorkout(this Data.Model.Workout w) => new WorkoutDto(
        w.Id,
        w.Owner,
        w.CreatorId,
        w.IsPublic,
        w.Name,
        w.Desciption,
        w.LastPerformed,
        w.Exercises.Select(e => e.ToExerciseDto())
    );

    public static Data.Model.Workout MapToWorkoutDto(this WorkoutDto w) => new Data.Model.Workout
    {
        Id = w.id,
        Owner = w.owner,
        CreatorId = w.creatorId,
        IsPublic = w.isPublic,
        Name = w.name,
        Desciption = w.description,
        LastPerformed = w.lastPerformed,
        Exercises = w.exercises.Select(e => e.ToExercise())
    };
}