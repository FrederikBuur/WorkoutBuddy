namespace WorkoutBuddy.Features;

public record ProfileDto(
    Guid id,
    string userId,
    string? name,
    string? email,
    string? profilePictureUrl
);
