using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Controllers.ExerciseModel;

namespace WorkoutBuddy.Controllers.WorkoutModel;

public static class WorkoutExtensions
{
    public static WorkoutDto ToWorkoutDto(this Workout w) => new(
        w.Id,
        w.Owner,
        w.CreatorId,
        w.Name,
        w.Description,
        w.IsPublic,
        w.LastPerformed,
        w.ExerciseEntries.Select(e => e.ToWorkoutExerciseEntryDto())
    );

    public static Workout ToWorkout(this WorkoutDto w)
    {
        var workout = new Workout()
        {
            Id = w.Id,
            Owner = w.Owner,
            CreatorId = w.CreatorId,
            Name = w.Name,
            Description = w.Description,
            IsPublic = w.IsPublic,
            LastPerformed = w.LastPerformed,
            ExerciseEntries = w.Exercises.Select(e => e.ToWorkoutExerciseEntry()).ToList()
        };
        return workout;
    }

    public static WorkoutExerciseEntryDto ToWorkoutExerciseEntryDto(this WorkoutExerciseEntry we) => new(
        we.Id,
        we.Order,
        we.ExerciseId
    );

    public static WorkoutExerciseEntry ToWorkoutExerciseEntry(this WorkoutExerciseEntryDto we) => new()
    {
        Id = we.Id,
        Order = we.Order,
        ExerciseId = we.ExerciseId
    };
}