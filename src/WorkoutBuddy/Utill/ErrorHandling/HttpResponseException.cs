using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features.ErrorHandling;

public class HttpResponseException : Exception
{
    public HttpResponseException(
        HttpStatusCode statusCode,
        string userFriendlyErrorDescription,
        object? value = null) =>
        (StatusCode, UserFriendlyErrorDescription, Value) = (statusCode, userFriendlyErrorDescription, value);

    public HttpStatusCode StatusCode { get; }
    public string? UserFriendlyErrorDescription { get; }
    public object? Value { get; }
}
