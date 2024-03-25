using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

public class CustomControllerBase : ControllerBase
{
    public ObjectResult GetDataOrError<T, R>(Result<T> result, Func<T, R> resolveResponse)
    {
        return result.Match(
                    (data) =>
                         Ok(resolveResponse(data)),
                    (err) => Problem(
                        title: err.Name,
                        statusCode: err.StatusCode,
                        detail: err.Description)
                );
    }
}