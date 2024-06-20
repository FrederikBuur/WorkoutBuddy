using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly ILogger<ProfilesController> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfilesService _profileService;

    public ProfilesController(ILogger<ProfilesController> logger, DataContext dataContext, ProfilesService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    // TODO CRUD operations for profile
}
