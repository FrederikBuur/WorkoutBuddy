using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Controllers.ExerciseModel;
using WorkoutBuddy.Features.WorkoutModel;

namespace WorkoutBuddy.Controllers.WorkoutModel;

[Authorize]
[ApiController]
[Route("api/workout")]
public class WorkoutController : ControllerBase
{
    private readonly ILogger<WorkoutController> _logger;
    private readonly DataContext _dataContext;
    private readonly WorkoutService _workoutService;

    public WorkoutController(
        ILogger<WorkoutController> logger,
        DataContext dataContext,
        WorkoutService workoutService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _workoutService = workoutService;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetWorkouts(
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
    public async Task<ActionResult<WorkoutDto>> GetWorkoutById([FromRoute][Required] Guid id)
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
    public async Task<ActionResult<WorkoutDto>> PostWorkout(
        [FromBody] WorkoutDto workoutDto
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
    public async Task<ActionResult<WorkoutDto>> PutWorkout([FromBody][Required] WorkoutDto workoutDto)
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
    public async Task<ActionResult<WorkoutDto>> DeleteWorkout([FromRoute][Required] Guid workoutId)
    {
        var workoutResult = await _workoutService.DeleteWorkoutDto(workoutId);
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
