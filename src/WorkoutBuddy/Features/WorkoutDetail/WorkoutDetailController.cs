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
    public async Task<ActionResult<IEnumerable<WorkoutDetailDto>>> GetWorkoutDetails(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var workoutDetailsResult = await _workoutDetailService.SearchWorkoutDetails(visibilityFilter, searchQuery, pageNumber, pageSize);
        return GetDataOrError(workoutDetailsResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDetailDto>> GetWorkoutDetailById([FromRoute][Required] Guid id)
    {
        var workoutDetailResult = await _workoutDetailService.GetWorkoutDetailById(id);
        return GetDataOrError(workoutDetailResult);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDetailDto>> PostWorkoutDetail(
        [FromBody] WorkoutDetailDto workoutDto
    )
    {
        var workoutDetailResult = await _workoutDetailService.CreateWorkoutDetail(workoutDto);
        return GetDataOrError(workoutDetailResult);
    }

    [HttpPut]
    public async Task<ActionResult<WorkoutDetailDto>> PutWorkoutDetail([FromBody][Required] WorkoutDetailDto workoutDto)
    {
        var workoutDetailResult = await _workoutDetailService.UpdateWorkoutDetail(workoutDto);
        return GetDataOrError(workoutDetailResult);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDetailDto>> DeleteWorkoutDetail([FromRoute][Required] Guid workoutDetailId)
    {
        var workoutDetailResult = await _workoutDetailService.DeleteWorkoutDetail(workoutDetailId);
        return GetDataOrError(workoutDetailResult);
    }

    // dublicate workout
}
