using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

public class CustomControllerBase : ControllerBase
{
    public ObjectResult GetDataOrError<T>(Result<T> result)
    {
        return result.Match(
                    (data) => Ok(data),
                    (err) => Problem(
                        statusCode: (int)err.StatusCode,
                        detail: err.UserFriendlyErrorDescription,
                        instance: err.Value?.ToString())
                );
    }
}