using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/workout-detail")]
public class WorkoutDetailController : CustomControllerBase
{
    private readonly ILogger<WorkoutDetailController> _logger;
    private readonly WorkoutDetailService _workoutDetailService;

    public WorkoutDetailController(
        ILogger<WorkoutDetailController> logger,
        WorkoutDetailService workoutService)
    {
        _logger = logger;
        _workoutDetailService = workoutService;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutDetailResponse>>> GetWorkoutDetails(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var workoutDetailsResult = await _workoutDetailService.SearchWorkoutDetails(visibilityFilter, searchQuery, pageNumber, pageSize);

        return GetDataOrError(
            result: workoutDetailsResult,
            resolveResponse: (wd) => wd.Select(e => new WorkoutDetailResponse(e))
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDetailResponse>> GetWorkoutDetailById(
        [FromRoute][Required] Guid id
    )
    {
        var workoutDetailResult = await _workoutDetailService.GetWorkoutDetailById(id);

        return GetDataOrError(
            result: workoutDetailResult,
            resolveResponse: (wd) => new WorkoutDetailResponse(wd)
            );
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDetailResponse>> CreateWorkoutDetail(
        [FromBody] CreateWorkoutDetailRequest createWorkoutRequest
    )
    {
        var workoutDetailResult = await _workoutDetailService.CreateWorkoutDetail(createWorkoutRequest);

        return GetDataOrError(
            result: workoutDetailResult,
            resolveResponse: (wd) => new WorkoutDetailResponse(wd)
            );
    }

    [HttpPut]
    public async Task<ActionResult<WorkoutDetailResponse>> UpdateWorkoutDetail(
        [FromBody][Required] UpdateWorkoutDetailRequest workout
    )
    {
        var workoutDetailResult = await _workoutDetailService.UpdateWorkoutDetail(workout);

        return GetDataOrError(
            result: workoutDetailResult,
            resolveResponse: (wd) => new WorkoutDetailResponse(wd)
            );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDetailResponse>> DeleteWorkoutDetail(
        [FromRoute][Required] Guid workoutDetailId
    )
    {
        var workoutDetailResult = await _workoutDetailService.DeleteWorkoutDetail(workoutDetailId);

        return GetDataOrError(
            result: workoutDetailResult,
            resolveResponse: (wd) => new WorkoutDetailResponse(wd)
            );
    }

    // dublicate workout
}
