using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

public static class WorkoutExtensions
{
    public static WorkoutDetailDto ToWorkoutDetailDto(this WorkoutDetail w) => new(
        w.Id,
        w.Owner,
        w.CreatorId,
        w.Name,
        w.Description,
        w.IsPublic,
        w.Exercises.Select(e => e.ToExerciseDetailDto())
    );

    public static WorkoutDetail ToWorkoutDetail(this WorkoutDetailDto w) => new(
        id: w.Id,
        owner: w.Owner,
        creatorId: w.CreatorId,
        name: w.Name,
        description: w.Description,
        isPublic: w.IsPublic);
}