using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

public static class WorkoutExtensions
{
    public static WorkoutDto ToWorkoutDto(this Workout w) => new(
        w.Id,
        w.LastPerformed,
        w.Count
    );

    public static Workout ToWorkout(this WorkoutDto w) => new(
        id: w.Id,
        lastPerformed: w.LastPerformed,
        count: w.Count);
}