using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Controllers.ExerciseModel;
using WorkoutBuddy.Features.WorkoutModel;
using System.Net;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Controllers.WorkoutModel;

[Authorize]
[ApiController]
[Route("api/workout")]
public class WorkoutController : ControllerBase
{
    private readonly ILogger<WorkoutController> _logger;
    private readonly DataContext _dataContext;
    private readonly WorkoutService _workoutService;
    private readonly IProfileService _profileService;

    public WorkoutController(
        ILogger<WorkoutController> logger,
        DataContext dataContext,
        WorkoutService workoutService,
        IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _workoutService = workoutService;
        _profileService = profileService;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<WorkoutDto>>> GetWorkouts(
        [FromQuery] VisibilityFilter visibilityFilter = VisibilityFilter.OWNED,
        [FromQuery] string? searchQuery = null,
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var workouts = await _workoutService.SearchWorkouts(visibilityFilter, searchQuery, pageNumber, pageSize);
        return Ok(workouts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutDto>> GetWorkoutById([FromRoute][Required] Guid id)
    {
        var workout = await _workoutService.GetWorkoutDtoById(id);
        return Ok(workout);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDto>> PostWorkout(
        [FromBody] WorkoutDto workoutDto
    )
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var w = workoutDto.ToWorkout();
        w.CreatorId = profile.Id;
        w.Owner = profile.Id;

        var result = _dataContext.Add(w);
        await _dataContext.SaveChangesAsync();
        var response = result.Entity.ToWorkoutDto();

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<WorkoutDto>> PutWorkout([FromBody][Required] WorkoutDto workoutDto)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var existingWorkout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutDto.id && w.Owner == profile.Id);
        if (existingWorkout is null)
            return NotFound("Workout not found");

        // update existing workout fields
        existingWorkout.Name = workoutDto.name;
        existingWorkout.Description = workoutDto.description;
        existingWorkout.IsPublic = workoutDto.isPublic;
        existingWorkout.ExerciseIds = workoutDto.exerciseIds;

        await _dataContext.SaveChangesAsync();

        return Ok(existingWorkout.ToWorkoutDto());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<WorkoutDto>> DeleteWorkout([FromRoute][Required] Guid workoutId)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            return Unauthorized();

        var workout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile.Id);
        if (workout is null)
            return NotFound("Workout not found");

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();

        return Ok(workout);
    }

    // dublicate workout
}
