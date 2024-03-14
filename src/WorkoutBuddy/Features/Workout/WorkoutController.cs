using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/workout")]
public class WorkoutController : CustomControllerBase
{
    private readonly ILogger<WorkoutController> _logger;
    private readonly WorkoutService _workoutService;

    public WorkoutController(
        ILogger<WorkoutController> logger,
        WorkoutService workoutService)
    {
        _logger = logger;
        _workoutService = workoutService;
    }

    // get all workouts by profileid from jwt token
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetAllWorkoutsByProfile()
    {
        // Get profileId from JWT token (you'll need to implement this part)
        var workoutsResult = await _workoutService.GetWorkoutsForProfile();
        return GetDataOrError(workoutsResult);
    }

    // get workout by id

    // delete workout by id 
    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDto>> DeleteWorkout([FromRoute][Required] Guid workoutId)
    {
        var workoutResult = await _workoutService.DeleteWorkout(workoutId);
        return GetDataOrError(workoutResult);
    }

    // create workoutset by workout id

    // create exerciseset by workoutset id

    // update exerciseset by workoutset id
}
