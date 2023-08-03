using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers.ExerciseModel;

[Authorize]
[ApiController]
[Route("api/exercise")]
public partial class ExerciseController : ControllerBase
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
    public async Task<ActionResult<List<ExerciseDto>>> GetExercises(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] MuscleGroupType? muscleGroupType = null,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var profileId = _profileService.GetProfile()?.Id ?? throw new Exception("Couldnt find Profile");

        #region predicates
        Expression<Func<Exercise, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileId
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileId);

        Expression<Func<Exercise, bool>> muscleGroupPredicate = (e) =>
            muscleGroupType != null
                &&
                e.MuscleGroups.ToLower().Contains(muscleGroupType!.ToString()!.ToLower() ?? "");

        Expression<Func<Exercise, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery)
            || e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var exercises = await _dataContext.Exercises
            .Where(visibilityPredicate)
            .Where(muscleGroupPredicate)
            .Where(searchQueryPredicate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = exercises.Select(e => e.ToExerciseDto());

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetExercise([FromRoute][Required] Guid id)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exercise = (await _dataContext.Exercises
            .SingleOrDefaultAsync(e => e.Id == id && e.Owner == profile.Id)
        )?.ToExerciseDto();

        if (exercise is null)
            return NotFound("Exercise not found");

        if (exercise?.isPublic != true && exercise?.owner != profile.Id)
            return Unauthorized();


        return Ok(exercise);
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> PostExercise([FromBody] ExerciseDto exerciseDto)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var e = exerciseDto.ToExercise();
        e.CreatorId = profile.Id;
        e.Owner = profile.Id;
        var result = _dataContext.Add(e);
        await _dataContext.SaveChangesAsync();
        var response = result.Entity.ToExerciseDto();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<ExerciseDto>> PutExercise(
        [FromBody] ExerciseDto exercise
    )
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var existingExercise = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == exercise.id);
        if (existingExercise is null)
            return NotFound("Exercise not found");

        if (profile.Id != existingExercise.Owner)
            return Unauthorized("Not allowed to update this exercise");

        existingExercise.IsPublic = exercise.isPublic;
        existingExercise.Name = exercise.name;
        existingExercise.Description = exercise.description;
        existingExercise.ImageUrl = exercise.imageUrl;
        existingExercise.MuscleGroups = exercise.muscleGroups;
        await _dataContext.SaveChangesAsync();

        return Ok(existingExercise.ToExerciseDto());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ExerciseDto>> DeleteExercise([FromRoute][Required] Guid id)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exerciseDto = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == id);
        if (exerciseDto is null)
            return NotFound("Exercise not found");

        if (profile.Id != exerciseDto.Owner)
            return Unauthorized();

        var result = _dataContext.Remove(exerciseDto);
        var response = result.Entity.ToExerciseDto();
        await _dataContext.SaveChangesAsync();
        return Ok(response);
    }
}