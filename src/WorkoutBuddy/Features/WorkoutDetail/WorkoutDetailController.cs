using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/workout-detail")]
public class WorkoutDetailController : ControllerBase
{
    private readonly ILogger<WorkoutDetailController> _logger;
    private readonly WorkoutDetailService _workoutService;

    public WorkoutDetailController(
        ILogger<WorkoutDetailController> logger,
        WorkoutDetailService workoutService)
    {
        _logger = logger;
        _workoutService = workoutService;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutDetailDto>>> GetWorkoutDetails(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var workoutsResult = await _workoutService.SearchWorkouts(visibilityFilter, searchQuery, pageNumber, pageSize);

        return workoutsResult.Match(
            (workouts) => Ok(workouts),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString())
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDetailDto>> GetWorkoutDetailById([FromRoute][Required] Guid id)
    {
        var workoutResult = await _workoutService.GetWorkoutDtoById(id);
        return workoutResult.Match(
            (workout) => Ok(workout),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString())
        );
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDetailDto>> PostWorkoutDetail(
        [FromBody] WorkoutDetailDto workoutDto
    )
    {
        var workoutResult = await _workoutService.CreateWorkoutDto(workoutDto);
        return workoutResult.Match(
            (workout) => Ok(workout),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString())
        );
    }

    [HttpPut]
    public async Task<ActionResult<WorkoutDetailDto>> PutWorkoutDetail([FromBody][Required] WorkoutDetailDto workoutDto)
    {
        var workoutResult = await _workoutService.UpdateWorkoutDto(workoutDto);
        return workoutResult.Match(
            (workout) => Ok(workout),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString())
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDetailDto>> DeleteWorkoutDetail([FromRoute][Required] Guid workoutDetailId)
    {
        var workoutResult = await _workoutService.DeleteWorkoutDto(workoutDetailId);
        return workoutResult.Match(
            (workout) => Ok(workout),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString())
        );
    }

    // dublicate workout
}
