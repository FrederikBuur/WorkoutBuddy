namespace WorkoutBuddy.Controllers.WorkoutModel;

public record WorkoutDto(
    Guid id,
    Guid owner,
    Guid creatorId,
    string name,
    string? description,
    bool isPublic,
    DateTime lastPerformed,
    IEnumerable<Guid> exerciseIds
);