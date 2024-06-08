using System.Text.Json.Serialization;

namespace WorkoutBuddy.Features.Authentication;

public record LoginRequest(
    string Email,
    string Password
);
