namespace WorkoutBuddy.Features;

public record ExerciseSetRequest(
    int Repetitions,
    double Weight,
    WeightUnit WeightUnit
);
