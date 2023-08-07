using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.WorkoutModel;

public record WorkoutDto(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    bool IsPublic,
    DateTime LastPerformed,
    IEnumerable<WorkoutExerciseEntryDto> Exercises
);

public record WorkoutExerciseEntryDto(
    Guid Id,
    int Order,
    Guid ExerciseId
);