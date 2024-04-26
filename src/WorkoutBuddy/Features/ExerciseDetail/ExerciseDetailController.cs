using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/exercise-detail")]
public partial class ExerciseDetailController : CustomControllerBase
{
    private readonly ILogger<ExerciseDetailController> _logger;
    private readonly ExerciseDetailService _exerciseService;
    public ExerciseDetailController(ILogger<ExerciseDetailController> logger, ExerciseDetailService exerciseService)
    {
        _logger = logger;
        _exerciseService = exerciseService;
    }
    [HttpGet]
    public async Task<ActionResult<GetExercisesResponse>> GetExercises(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] MuscleGroupType? muscleGroupType = null,
        [FromQuery] string? searchQuery = null,
        [FromQuery][Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery][Range(1, int.MaxValue)] int pageSize = 10
    )
    {
        var validationList = new List<(bool, string)> {
             (pageNumber < 1, "Page number cannot be lower that 1"),
             (pageSize < 1, "Page size cannot be lower that 1")
             };
        if (validationList.ContainsValidationErrors(out var badRequestObjectResult))
        {
            return badRequestObjectResult;
        }

        var paginatedExercises = await _exerciseService.GetExercisesAsync(
            visibilityFilter,
            muscleGroupType,
            searchQuery,
            pageNumber,
            pageSize
        );

        return paginatedExercises.ToActionResult(p => new GetExercisesResponse(p));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDetailResponse>> GetExercise([FromRoute][Required] Guid id)
    {
        var exercise = await _exerciseService.GetExerciseAsync(id);

        return exercise.ToActionResult(e => new ExerciseDetailResponse(e));
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDetailResponse>> CreateExercise(
        [FromBody] CreateExerciseDetailRequest createExerciseRequest
        )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(createExerciseRequest);

        // return GetDataOrError(
        //     result: exercise,
        //     resolveResponse: (ed) => new ExerciseDetailResponse(ed)
        // );
        return exercise.ToActionResult(ed => new ExerciseDetailResponse(ed));
    }

    [HttpPut]
    public async Task<ActionResult<ExerciseDetailResponse>> UpdateExercise(
        [FromBody] UpdateExerciseDetailRequest exercise
    )
    {
        var updatedExercise = await _exerciseService.UpdateExerciseAsync(exercise);

        return GetDataOrError(
            updatedExercise,
            (ed) => new ExerciseDetailResponse(ed)
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ExerciseDetailResponse>> DeleteExercise(
        [FromRoute][Required] Guid exerciseId)
    {
        var deletedExercise = await _exerciseService.DeleteExerciseAsync(exerciseId);

        return GetDataOrError(
            deletedExercise,
            (ed) => new ExerciseDetailResponse(ed)
        );
    }
}
