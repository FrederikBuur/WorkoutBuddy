namespace WorkoutBuddy.Features;

public record ExerciseLogRequest(
    Guid ExerciseDetailId,
    List<ExerciseSetRequest> ExerciseSetsRequest
);
