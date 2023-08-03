using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Controllers.ExerciseModel;

namespace WorkoutBuddy.Controllers.WorkoutModel;

public static class WorkoutExtensions
{
    public static WorkoutDto ToWorkoutDto(this Workout w) => new WorkoutDto(
        w.Id,
        w.Owner,
        w.CreatorId,
        w.Name,
        w.Description,
        w.IsPublic,
        w.LastPerformed,
        w.ExerciseIds
    );

    public static Workout ToWorkout(this WorkoutDto w) => new Workout(
        w.id,
        w.owner,
        w.creatorId,
        w.name,
        w.description,
        w.isPublic,
        w.lastPerformed,
        w.exerciseIds
    );
}