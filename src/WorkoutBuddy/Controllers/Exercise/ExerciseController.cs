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
        var profileId = _profileService.GetProfile()?.Id ?? throw new Exception("Couldnt finde frofile");

        #region predicates
        Expression<Func<Data.Model.Exercise, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileId
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileId);

        Expression<Func<Data.Model.Exercise, bool>> muscleGroupPredicate = (e) =>
            muscleGroupType != null
            && String.Join(",", nameof(e.MuscleGroups)).Contains(nameof(muscleGroupType));

        Expression<Func<Data.Model.Exercise, bool>> searchQueryPredicate = (e) =>
            !string.IsNullOrWhiteSpace(searchQuery)
            && e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var exercises = await _dataContext.Exercises
            //.Where(visibilityPredicate) // working
            .Where(e =>
                muscleGroupType != null
                &&
                e.MuscleGroups.ToLower().Contains(muscleGroupType!.ToString()!.ToLower() ?? "")
            )
            //.Where(searchQueryPredicate) // working
            .Skip(pageNumber * pageNumber)
            .Take(pageSize)
            .ToListAsync();

        var result = exercises.Select(e => e.ToExerciseDto());

        return Ok(result);
    }

    private bool qwerty(Exercise e, MuscleGroupType? muscleGroupType)
    {
        var ok = muscleGroupType != null
                && e.MuscleGroups.Any(mg => nameof(mg) == nameof(muscleGroupType));
        return ok;
    }

    private Func<Tuple<Guid, VisibilityFilter>, Expression<Func<Data.Model.Exercise, bool>>> testFilter = (tuple) =>
    {
        (Guid profileId, VisibilityFilter filter) = tuple;
        return ((e) =>
            (filter == VisibilityFilter.PUBLIC && e.IsPublic)
            || filter == VisibilityFilter.OWNED && e.Owner == profileId
            || filter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileId));
    };

    [HttpGet("{id}")]
    public ActionResult<Exception> GetExercise([FromRoute][Required] Guid id)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exercise = _dataContext.Exercises
            .FirstOrDefault(e => e.Id == id)
            ?.ToExerciseDto();

        if (exercise?.isPublic != true && exercise?.owner != profile.Id)
            return Unauthorized();

        if (exercise is null)
            return NotFound("Exercise not found");

        return Ok(exercise);
    }

    [HttpPost]
    public ActionResult<ExerciseDto> PostExercise([FromBody] ExerciseDto exercise)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var e = exercise.ToExercise();
        e.CreatorId = profile.Id;
        var result = _dataContext.Add(e);
        _dataContext.SaveChanges();
        var response = result.Entity.ToExerciseDto();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<ExerciseDto>> PutExercise(
        [FromBody] ExerciseDto updatedExercise
    )
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var exerciseDto = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == updatedExercise.id);
        if (exerciseDto is null)
            return NotFound("Exercise not found");

        if (profile.Id != exerciseDto.Owner)
            return Unauthorized("Not allowed to update this exercise");

        exerciseDto.IsPublic = updatedExercise.isPublic;
        exerciseDto.Name = updatedExercise.name;
        exerciseDto.Description = updatedExercise.description;
        exerciseDto.ImageUrl = updatedExercise.imageUrl;
        exerciseDto.MuscleGroups = updatedExercise.muscleGroups;
        await _dataContext.SaveChangesAsync();

        return Ok(exerciseDto.ToExerciseDto());
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