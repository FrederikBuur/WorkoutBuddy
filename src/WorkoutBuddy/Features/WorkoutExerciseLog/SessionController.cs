
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/session")]
public class SessionController : ControllerBase
{
    private readonly ILogger<WorkoutDetailController> _logger;
    private readonly SessionService _sessionService;

    public SessionController(
        ILogger<WorkoutDetailController> logger,
        SessionService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    [HttpGet("workout/{id}")]
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

    [HttpGet("workout-log/{id}")]
    public async Task<ActionResult<WorkoutLogResponse>> GetWorkoutLogById(
        Guid id
    )
    {
        var workoutLogResult = await _sessionService.GetWorkoutLogById(id);

        return workoutLogResult.ToActionResult(wl => new WorkoutLogResponse(wl));
    }

    [HttpPost("workout-log")]
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
