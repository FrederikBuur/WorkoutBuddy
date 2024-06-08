namespace WorkoutBuddy.Features.Authentication;

public record RefreshJwtRequest(
    string RefreshToken
);