using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Data.Model;

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
    public async Task<ActionResult<List<ExerciseDetailResponse>>> GetExercises(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] MuscleGroupType? muscleGroupType = null,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var exercises = await _exerciseService.GetExercisesAsync(
            visibilityFilter,
            muscleGroupType,
            searchQuery,
            pageNumber,
            pageSize
        );

        return GetDataOrError(
            exercises,
            (e) => e.Select((ed) => new ExerciseDetailResponse(ed))
        );

        // ERROR handling: https://www.youtube.com/watch?v=YBK93gkGRj8&ab_channel=MilanJovanovi%C4%87
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDetailResponse>> GetExercise([FromRoute][Required] Guid id)
    {
        var exercise = await _exerciseService.GetExerciseAsync(id);

        return GetDataOrError(
            exercise,
            (ed) => new ExerciseDetailResponse(ed)
        );
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDetailResponse>> CreateExercise(
        [FromBody] CreateExerciseDetailRequest createExerciseRequest
        )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(createExerciseRequest);

        return GetDataOrError(
            result: exercise,
            resolveResponse: (ed) => new ExerciseDetailResponse(ed)
        );
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
