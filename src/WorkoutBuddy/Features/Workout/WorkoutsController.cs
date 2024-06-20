using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/workouts")]
public class WorkoutsController : CustomControllerBase
{
    private readonly ILogger<WorkoutsController> _logger;
    private readonly WorkoutsService _workoutService;

    public WorkoutsController(
        ILogger<WorkoutsController> logger,
        WorkoutsService workoutService)
    {
        _logger = logger;
        _workoutService = workoutService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<WorkoutResponse>>> GetAllWorkoutsByProfile()
    {
        var workoutsResult = await _workoutService.GetWorkoutsForProfile();

        return workoutsResult.ToActionResult((w) =>
            w.Select(i => new WorkoutResponse(i)).ToList()
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutResponse>> GetWorkoutById([FromRoute] Guid id)
    {
        var workoutResult = await _workoutService.GetWorkoutById(id);

        return workoutResult.ToActionResult((w) => new WorkoutResponse(w));
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutResponse>> PostWorkout(
        [FromBody] CreateWorkoutRequest workoutRequest)
    {
        var workoutRepsonse = await _workoutService.CreateWorkout(workoutRequest);

        return workoutRepsonse.ToActionResult((w) => new WorkoutResponse(w));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutResponse>> DeleteWorkout([FromRoute] Guid id)
    {
        var workoutResult = await _workoutService.DeleteWorkout(id);

        return workoutResult.ToActionResult((w) => new WorkoutResponse(w));
    }
}
