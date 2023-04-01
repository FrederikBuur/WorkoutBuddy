namespace WorkoutBuddy.Controllers.Exercise.Model;

public record Profile(
    Guid id, 
    string userId, 
    string? name, 
    string? email, 
    string? profilePictureUrl
);
