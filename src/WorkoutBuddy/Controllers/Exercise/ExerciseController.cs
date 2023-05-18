using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorkoutBuddy.Controllers.Exercise.Model;

[Authorize]
[ApiController]
[Route("api/exercise")]
public class ExerciseController : ControllerBase
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly DataContext _dataContext;
    private readonly IProfileService _profileService;

    public ExerciseController(ILogger<ExerciseController> logger, DataContext dataContext, IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Exercise>>> GetExercises()
    {
        var exercises = await _dataContext.Exercises
            .ToListAsync();
        var result = exercises.Select(e => e.ToExercise());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<Exception> GetExercise([FromRoute][Required] Guid id)
    {
        var exercise = _dataContext.Exercises
            .FirstOrDefault(e => e.Id == id)
            ?.ToExercise();

        if (exercise is null)
            return NotFound("Exercise not found");

        return Ok(exercise);
    }

    [HttpPost]
    public ActionResult<Exercise> PostExercise([FromBody] Exercise exercise)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var e = exercise.ToExerciseDto();
        e.CreatorId = profile.Id;
        var result = _dataContext.Add(e);
        _dataContext.SaveChanges();
        var response = result.Entity.ToExercise();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<Exercise>> PutExercise(
        [FromBody] Exercise updatedExercise
    )
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exerciseDto = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == updatedExercise.id);
        if (exerciseDto is null)
            return NotFound("Exercise not found");

        if (profile.Id != exerciseDto.CreatorId)
            return Unauthorized("Not allowed to update this exercise");

        exerciseDto.Name = updatedExercise.name;
        exerciseDto.Description = updatedExercise.description;
        exerciseDto.ImageUrl = updatedExercise.imageUrl;
        exerciseDto.PrimaryMuscleGroup = updatedExercise.primaryMuscleGroup;
        exerciseDto.SecondaryMuscleGroups = updatedExercise.secondaryMuscleGroup;
        await _dataContext.SaveChangesAsync();

        return Ok(exerciseDto.ToExercise());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Exercise>> DeleteExercise([FromRoute][Required] Guid id)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exerciseDto = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == id);
        if (exerciseDto is null)
            return NotFound("Exercise not found");

        if (profile.Id != exerciseDto.CreatorId)
            return Unauthorized("Not allowed to delete this exercise");

        var result = _dataContext.Remove(exerciseDto);
        var response = result.Entity.ToExercise();
        await _dataContext.SaveChangesAsync();
        return Ok(response);
    }

}