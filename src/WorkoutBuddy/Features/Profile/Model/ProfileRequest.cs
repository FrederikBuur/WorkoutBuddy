namespace WorkoutBuddy.Features;

public record ProfileRequest(
    string UserId,
    string? Name,
    string? Email,
    string? ProfilePictureUrl
);
