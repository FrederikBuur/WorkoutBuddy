using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workouts.Util;

namespace workouts.Controllers;

[Authorize]
[ApiController]
[Route("api/exercise")]
public class ExerciseController : ControllerBase
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly DataContext _dataContext;

    public ExerciseController(ILogger<ExerciseController> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Exercise>>> GetExercises()
    {
        var user = User;    
        var exercises = await _dataContext.Exercises
            .ToListAsync();
        var result = exercises.Select(e => e.ToExercise());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<Exception> GetExercise([FromRoute] Guid id)
    {
        var exercise = _dataContext.Exercises
            .Include(e => e.MuscleGroups)
            .FirstOrDefault(e => e.Id == id)
            ?.ToExercise();
        //return Ok(new Exercise(1, 1, "Bench", "Lift", null, new[] { MuscelGroup.Chest, MuscelGroup.UpperBack } ));

        if (exercise is null)
        {
            return NotFound("Exercise not found");
        }
        else
        {
            return Ok(exercise);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> PostExercise([FromBody] Exercise exercise)
    {
        // save exercise
        var e = exercise.ToExerciseDto();
        var result = _dataContext.Add(e);
        var response = result.Entity.ToExercise();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<Exercise>> PutExercise([FromBody] Exercise updatedExercise)
    {
        /*var exerciseDto = exercisesInMemoryDb.SingleOrDefault(e => e.Id == updatedExercise.id);
        if (exerciseDto is null)
        {
            return NotFound("Exercise not found");
        }
        else
        {
            exerciseDto.CreaterId = updatedExercise.creatorId;
            exerciseDto.Name = updatedExercise.name;
            exerciseDto.Description = updatedExercise.description;
            exerciseDto.ImageUrl = updatedExercise.imageUrl;
            exerciseDto.MuscleGroups = updatedExercise.muscleGroups.Select(mg => mg.ToMuscleGroupDto());

            return Ok(exerciseDto.ToExercise());
        }*/
        return BadRequest("not implemented");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Exercise>> DeleteExercise(Guid id)
    {
        var exerciseDto = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == id);

        if (exerciseDto is null)
        {
            return NotFound("Exercise not found");
        }
        else
        {
            var headerId = User.Identity?.Name;
            var creatorId = exerciseDto.CreaterId.ToString();
            if (headerId == creatorId)
            {
                var result = _dataContext.Remove(exerciseDto);
                var response= result.Entity.ToExercise();
                return Ok(response);
            }
            else
            {
                return Unauthorized("You are not autherized to delete this exercise");
            }
        }
    }

}