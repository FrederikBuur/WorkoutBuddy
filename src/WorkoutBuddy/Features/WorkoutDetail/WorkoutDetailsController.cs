using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/workout-details")]
public class WorkoutDetailsController : CustomControllerBase
{
    private readonly ILogger<WorkoutDetailsController> _logger;
    private readonly WorkoutDetailsService _workoutDetailService;

    public WorkoutDetailsController(
        ILogger<WorkoutDetailsController> logger,
        WorkoutDetailsService workoutService)
    {
        _logger = logger;
        _workoutDetailService = workoutService;
    }

    [HttpGet()]
    public async Task<ActionResult<GetWorkoutDetailsResponse>> GetWorkoutDetails(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var paignatedWorkoutDetialsResult = await _workoutDetailService.SearchWorkoutDetails(
            visibilityFilter,
            searchQuery,
            pageNumber,
            pageSize
        );

        return paignatedWorkoutDetialsResult.ToActionResult(p => new GetWorkoutDetailsResponse(p));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDetailResponse>> GetWorkoutDetailById(
        [FromRoute][Required] Guid id
    )
    {
        var workoutDetailResult = await _workoutDetailService.GetWorkoutDetailById(id);

        return workoutDetailResult.ToActionResult((wd) => new WorkoutDetailResponse(wd));
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDetailResponse>> CreateWorkoutDetail(
        [FromBody] CreateWorkoutDetailRequest createWorkoutRequest
    )
    {
        var workoutDetailResult = await _workoutDetailService.CreateWorkoutDetail(createWorkoutRequest);

        return workoutDetailResult.ToActionResult((wd) => new WorkoutDetailResponse(wd));
    }

    [HttpPut]
    public async Task<ActionResult<WorkoutDetailResponse>> UpdateWorkoutDetail(
        [FromBody][Required] UpdateWorkoutDetailRequest workout
    )
    {
        var workoutDetailResult = await _workoutDetailService.UpdateWorkoutDetail(workout);

        return workoutDetailResult.ToActionResult((wd) => new WorkoutDetailResponse(wd));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDetailResponse>> DeleteWorkoutDetail(
        [FromRoute][Required] Guid workoutDetailId
    )
    {
        var workoutDetailResult = await _workoutDetailService.DeleteWorkoutDetail(workoutDetailId);

        return workoutDetailResult.ToActionResult((wd) => new WorkoutDetailResponse(wd));
    }

    // dublicate workout if nesccesary
}
