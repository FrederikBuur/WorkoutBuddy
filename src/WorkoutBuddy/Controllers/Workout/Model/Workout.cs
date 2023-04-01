namespace WorkoutBuddy.Controllers.Workout.Model;

public record Workout(
    Guid id, 
    Guid owner,
    Guid creatorId,
    bool isPublic,
    string name, 
    string? description, 
    DateTime lastPerformed, 
    IEnumerable<Guid> exerciseIds
);