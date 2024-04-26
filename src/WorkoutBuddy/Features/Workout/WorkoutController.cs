using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

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

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutResponse>>> GetAllWorkoutsByProfile()
    {
        var workoutsResult = await _workoutService.GetWorkoutsForProfile();

        return GetDataOrError(
            result: workoutsResult,
            resolveResponse: (w) => w.Select(e => new WorkoutResponse(e))
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutResponse>> GetWorkoutById([FromRoute][Required] Guid workoutId)
    {
        var workoutResult = await _workoutService.GetWorkoutById(workoutId);

        return GetDataOrError(
            result: workoutResult,
            resolveResponse: (w) => new WorkoutResponse(w)
        );
    }

    // delete workout by id 
    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutResponse>> DeleteWorkout([FromRoute][Required] Guid workoutId)
    {
        var workoutResult = await _workoutService.DeleteWorkout(workoutId);

        return GetDataOrError(
            result: workoutResult,
            resolveResponse: (w) => new WorkoutResponse(w)
        );
    }

    // create workoutset by workout id

    // create exerciseset by workoutset id

    // update exerciseset by workoutset id
}
