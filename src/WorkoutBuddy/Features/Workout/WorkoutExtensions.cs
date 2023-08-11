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

    public static Workout ToWorkout(this WorkoutDto w) => new(
        id: w.Id,
        owner: w.Owner,
        creatorId: w.CreatorId,
        name: w.Name,
        description: w.Description,
        isPublic: w.IsPublic,
        lastPerformed: w.LastPerformed,
        exerciseEntries: w.Exercises.Select(e => e.ToWorkoutExerciseEntry()).ToList());

    public static WorkoutExerciseEntryDto ToWorkoutExerciseEntryDto(this WorkoutExerciseEntry we) => new(
        we.Id,
        we.Order,
        we.ExerciseId
    );

    public static WorkoutExerciseEntry ToWorkoutExerciseEntry(this WorkoutExerciseEntryDto we) => new(
        id: we.Id,
        order: we.Order,
        exerciseId: we.ExerciseId
    );
}