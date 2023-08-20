using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly ILogger<ProfileController> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;

    public ProfileController(ILogger<ProfileController> logger, DataContext dataContext, ProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    // TODO CRUD operations for profile
}
