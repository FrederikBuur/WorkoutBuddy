
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly ILogger<WorkoutDetailsController> _logger;
    private readonly SessionsService _sessionService;

    public SessionsController(
        ILogger<WorkoutDetailsController> logger,
        SessionsService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    [HttpGet("workouts/{id}")]
    public async Task<ActionResult<GetSessionsResponse>> GetSessionsForWorkout(
        Guid id,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
        )
    {
        var sessionsPageResult = await _sessionService.GetWorkoutLogsForWorkoutPaginated(
            id,
            pageNumber,
            pageSize);

        return sessionsPageResult.ToActionResult(p => new GetSessionsResponse(id, p));
    }

    [HttpGet("workout-logs/{id}")]
    public async Task<ActionResult<WorkoutLogResponse>> GetWorkoutLogById(
        Guid id
    )
    {
        var workoutLogResult = await _sessionService.GetWorkoutLogById(id);

        return workoutLogResult.ToActionResult(wl => new WorkoutLogResponse(wl));
    }

    [HttpPost("workout-logs")]
    public async Task<ActionResult<WorkoutLogResponse>> CreateWorkoutLog(
        [FromBody] WorkoutLogRequest workoutLogRequest
    )
    {
        var workoutLogResult = await _sessionService.CreateWorkoutLog(workoutLogRequest);

        return workoutLogResult.ToActionResult(wl => new WorkoutLogResponse(wl));
    }

    // delete workoutlog/exerciselog/exerciseset

    // update workoutlog/exerciselog/exerciseset
}
