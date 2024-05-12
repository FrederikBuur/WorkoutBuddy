namespace WorkoutBuddy.Features;

public record WorkoutLogRequest(
    Guid WorkoutId,
    List<ExerciseLogRequest> ExerciseLogsRequest
);
