
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.Workout.Model;

public static class WorkoutExtensions 
{
    public static Workout MapToWorkout(this WorkoutDto w) => new Workout(
        w.Id, 
        w.Owner, 
        w.CreatorId, 
        w.IsPublic, 
        w.Name, 
        w.Desciption, 
        w.LastPerformed, 
        w.ExerciseIds
    );

    public static WorkoutDto MapToWorkoutDto(this Workout w) => new WorkoutDto
    {
        Id = w.id, 
        Owner = w.owner, 
        CreatorId = w.creatorId, 
        IsPublic = w.isPublic, 
        Name = w.name, 
        Desciption = w.description, 
        LastPerformed = w.lastPerformed, 
        ExerciseIds = w.exerciseIds
    };
}