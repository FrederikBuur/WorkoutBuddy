namespace WorkoutBuddy.Features;

public record CreateWorkoutRequest(
    string Name,
    Guid WorkoutDetailId
);