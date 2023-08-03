using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features.ErrorHandling;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is HttpResponseException httpResponseException)
        {
            return Problem(
                statusCode: (int)httpResponseException.StatusCode,
                detail: httpResponseException.Value?.ToString());
        }

        return Problem(
            statusCode: (int)HttpStatusCode.InternalServerError
        );
    }
}