namespace WorkoutBuddy.Controllers.Workout.Model;

public record WorkoutDto(
    Guid id,
    Guid owner,
    Guid creatorId,
    bool isPublic,
    string name,
    string? description,
    DateTime lastPerformed,
    IEnumerable<WorkoutBuddy.Controllers.Exercise.Model.ExerciseDto> exercises
);