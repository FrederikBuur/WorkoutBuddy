using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Controllers.Workout;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;

    public ProfileController(ILogger<ProfileController> logger, ProfileService profileService)
    {
        _logger = logger;
        _profileService = profileService;
    }

    // TODO CRUD operations for profile
}
