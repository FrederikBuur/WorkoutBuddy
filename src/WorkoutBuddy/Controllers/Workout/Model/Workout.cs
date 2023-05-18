using WorkoutBuddy.Controllers.Exercise.Model;

namespace WorkoutBuddy.Controllers.Workout.Model;

public record Workout(
    Guid id,
    Guid owner,
    Guid creatorId,
    bool isPublic,
    string name,
    string? description,
    DateTime lastPerformed,
    IEnumerable<WorkoutBuddy.Controllers.Exercise.Model.Exercise> exercises
);