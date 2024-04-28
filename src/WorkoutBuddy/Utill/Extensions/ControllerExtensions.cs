
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Util;

public static class ControllerExtensions
{
    public static ActionResult<R> ToActionResult<T, R>(
        this Result<T> result, Func<T, R> resolveResponse)
    {
        return result.Match(
                    (data) =>
                    {
                        var response = resolveResponse(data);
                        var res = new OkObjectResult(response);
                        if (response?.GetType() != typeof(R))
                        {
                            throw new ArgumentException($"Return type \"${response?.GetType()}\" does not match return type \"${typeof(R)}\"");
                        }
                        return res;

                    },
                    (err) =>
                    {
                        var response = new ObjectResult(new
                        {
                            title = err.Name,
                            detail = err.Description
                        })
                        {
                            StatusCode = err.StatusCode
                        };
                        return response;
                    }
                );
    }

    public static bool ContainsValidationErrors(this List<(bool contidion, string description)> errorContidions, out BadRequestObjectResult? badRequestObjectResult)
    {
        var errors = errorContidions.Where(ec => ec.contidion)
        .Select(ec => ec.description).ToList();

        if (errors?.Any() is true)
        {
            badRequestObjectResult = new BadRequestObjectResult(errors);
            return true;
        }
        else
        {
            badRequestObjectResult = null;
            return false;
        }

    }
}