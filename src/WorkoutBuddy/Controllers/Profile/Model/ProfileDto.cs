namespace WorkoutBuddy.Controllers.ExerciseModel;

public record ProfileDto(
    Guid id,
    string userId,
    string? name,
    string? email,
    string? profilePictureUrl
);
