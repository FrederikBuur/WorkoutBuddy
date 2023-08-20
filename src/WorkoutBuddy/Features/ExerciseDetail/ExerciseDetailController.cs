using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

[Authorize]
[ApiController]
[Route("api/exercise-detail")]
public partial class ExerciseDetailController : ControllerBase
{
    private readonly ILogger<ExerciseDetailController> _logger;
    private readonly DataContext _dataContext;
    private readonly ExerciseService _exerciseService;
    public ExerciseDetailController(ILogger<ExerciseDetailController> logger, DataContext dataContext, ExerciseService exerciseService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _exerciseService = exerciseService;
    }
    [HttpGet]
    public async Task<ActionResult<List<ExerciseDetailDto>>> GetExercises(
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

        return exercises.Match(
            (exercises) => Ok(exercises),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString()
            )
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDetail>> GetExercise([FromRoute][Required] Guid id)
    {
        var exercise = await _exerciseService.GetExerciseAsync(id);

        return exercise.Match(
            (exercise) => Ok(exercise),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString()
            )
        );
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDetailDto>> PostExercise([FromBody] ExerciseDetailDto exerciseDto)
    {
        var exercise = await _exerciseService.CreateExerciseAsync(exerciseDto);

        return exercise.Match(
            (exercise) => Ok(exercise),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString()
            )
        );
    }

    [HttpPut]
    public async Task<ActionResult<ExerciseDetailDto>> PutExercise(
        [FromBody] ExerciseDetailDto exercise
    )
    {
        var updatedExercise = await _exerciseService.UpdateExerciseAsync(exercise);

        return updatedExercise.Match(
            (exercise) => Ok(exercise),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString()
            )
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ExerciseDetailDto>> DeleteExercise(
        [FromRoute][Required] Guid exerciseId)
    {
        var deletedExercise = await _exerciseService.DeleteExerciseAsync(exerciseId);

        return deletedExercise.Match(
            (exercise) => Ok(exercise),
            (err) => Problem(
                statusCode: (int)err.StatusCode,
                detail: err.UserFriendlyErrorDescription,
                instance: err.Value?.ToString()
            )
        );
    }
}