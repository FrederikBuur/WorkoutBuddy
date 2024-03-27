namespace WorkoutBuddy.Features;

public record CreateWorkoutDetailRequest(
    string Name,
    string? Description,
    bool IsPublic,
    IEnumerable<Guid> ExerciseIds
);
