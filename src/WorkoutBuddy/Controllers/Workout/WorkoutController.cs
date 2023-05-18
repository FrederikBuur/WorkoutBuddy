using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Controllers.Workout.Model;

[Authorize]
[ApiController]
[Route("api/workout")]
public class WorkoutController : ControllerBase
{
    private readonly ILogger<WorkoutController> _logger;
    private readonly DataContext _dataContext;
    private readonly IProfileService _profileService;

    public WorkoutController(ILogger<WorkoutController> logger, DataContext dataContext, IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<WorkoutDto>> GetWorkouts(
        [FromQuery] bool publicWorkouts = false
    )
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var workouts = _dataContext.Workouts.Where(w =>
            publicWorkouts
                ? w.IsPublic
                : w.Owner == profile.Id
        );
        var response = workouts.Select(w => w.MapToWorkout());

        return Ok(response);
    }

    // serach workouts with filters

    // get workout by id

    // post new workout

    // dublicate workout

    // put workout

    // delete workout
}
