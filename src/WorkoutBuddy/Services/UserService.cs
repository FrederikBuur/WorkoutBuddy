
using System.Security.Claims;

namespace WorkoutBuddy.Services;

public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    private ClaimsPrincipal? User => HttpContext?.User;

    public string? Id => User?.FindFirstValue("id");

    public string? Email => User?.FindFirstValue("email");

}