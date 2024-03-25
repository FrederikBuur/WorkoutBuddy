using System.Net;

namespace WorkoutBuddy.Features.ErrorHandling;

public record Error
{
    private Error(string name, string description, int statusCode)
    {
        Name = name;
        Description = description;
        StatusCode = statusCode;
    }

    public string Name { get; }
    public string Description { get; }
    public int StatusCode { get; }

    public static Error Unauthorized(string description) =>
        new("Unauthorized", description, StatusCodes.Status401Unauthorized);

    public static Error BadRequest(string description) =>
        new("Bad Request", description, StatusCodes.Status400BadRequest);

    public static Error NotFound(string description) =>
        new("Not Found", description, StatusCodes.Status404NotFound);

    public static Error Conflict(string description) =>
        new("Conflict", description, StatusCodes.Status409Conflict);
}