using System.Net;

namespace WorkoutBuddy.Features.ErrorHandling;

[Obsolete("Use Error type instead")]
public class HttpResponceException : Exception
{
    public HttpResponceException(
        HttpStatusCode statusCode,
        string userFriendlyErrorDescription,
        object? value = null) =>
        (StatusCode, UserFriendlyErrorDescription, Value) = (statusCode, userFriendlyErrorDescription, value);

    public HttpStatusCode StatusCode { get; }
    public string? UserFriendlyErrorDescription { get; }
    public object? Value { get; }
}
