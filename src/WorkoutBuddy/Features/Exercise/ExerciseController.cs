using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ExerciseModel;
using WorkoutBuddy.Features.WorkoutModel;

namespace WorkoutBuddy.Controllers.ExerciseModel;

[Authorize]
[ApiController]
[Route("api/exercise")]
public partial class ExerciseController : ControllerBase
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly DataContext _dataContext;
    private readonly ExerciseService _exerciseService;
    public ExerciseController(ILogger<ExerciseController> logger, DataContext dataContext, ExerciseService exerciseService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _exerciseService = exerciseService;
    }
    [HttpGet]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercises(
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
    public async Task<ActionResult<Exercise>> GetExercise([FromRoute][Required] Guid id)
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
    public async Task<ActionResult<ExerciseDto>> PostExercise([FromBody] ExerciseDto exerciseDto)
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
    public async Task<ActionResult<ExerciseDto>> PutExercise(
        [FromBody] ExerciseDto exercise
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
    public async Task<ActionResult<ExerciseDto>> DeleteExercise(
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