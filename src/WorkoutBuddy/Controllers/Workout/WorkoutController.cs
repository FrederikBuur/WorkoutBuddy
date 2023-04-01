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

    [HttpGet("by-user")]
    public ActionResult<IEnumerable<Workout>> GetWorkoutsByUser(
        [FromQuery][Required] Guid profileId
    )
    {
        var idClaim = User.Claims.SingleOrDefault(c => c.Type == "id")?.Value;
        if (idClaim is null)
            return Unauthorized("Id missing in claims");

        var profile = _profileService.GetProfileByUserId(idClaim);
        if (profile is null)
            return Unauthorized("Invalid claim id");

        var workouts = _dataContext.Workouts.Where(w =>
            w.Owner == profileId &&
            profileId == profile.Id
                ? true
                : w.IsPublic
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
