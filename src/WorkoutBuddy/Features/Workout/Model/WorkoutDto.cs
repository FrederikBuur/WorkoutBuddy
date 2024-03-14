namespace WorkoutBuddy.Features;

public record WorkoutDto(
    Guid Id,
    DateTime LastPerformed,
    int Count
);