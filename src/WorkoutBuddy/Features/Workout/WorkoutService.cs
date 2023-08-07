using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Controllers;
using WorkoutBuddy.Controllers.ExerciseModel;
using WorkoutBuddy.Controllers.WorkoutModel;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features.WorkoutModel;

public class WorkoutService
{
    private readonly ILogger<WorkoutService> _logger;
    private readonly DataContext _dataContext;
    private readonly IProfileService _profileService;

    public WorkoutService(ILogger<WorkoutService> logger,
    DataContext dataContext,
    IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<WorkoutDto>>> SearchWorkouts(
        VisibilityFilter visibilityFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<IEnumerable<WorkoutDto>>(profileErr);


        #region predicates
        Expression<Func<Workout, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profile!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profile!.Id);

        Expression<Func<Workout, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery)
            || e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var workouts = await _dataContext.Workouts
            .Where(visibilityPredicate)
            .Where(searchQueryPredicate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return workouts.Select(w => w.ToWorkoutDto()).ToList();
    }

    public async Task<Result<WorkoutDto>> GetWorkoutDtoById(Guid workoutId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDto>(profileErr);

        var workout = (await _dataContext.Workouts
            .SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile!.Id))
            ?.ToWorkoutDto();

        if (workout is null)
            return new Result<WorkoutDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (workout?.IsPublic != true && workout?.Owner != profile!.Id)
            return new Result<WorkoutDto>(
                new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have access to this workout")
            );

        return workout;
    }

    public async Task<Result<WorkoutDto>> CreateWorkoutDto(WorkoutDto workoutDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDto>(profileErr);

        var w = workoutDto.ToWorkout();
        w.CreatorId = profile?.Id ?? default;
        w.Owner = profile?.Id ?? default;

        var result = _dataContext.Add(w);
        await _dataContext.SaveChangesAsync();
        return result.Entity.ToWorkoutDto();
    }

    public async Task<Result<WorkoutDto>> UpdateWorkoutDto(WorkoutDto workoutDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDto>(profileErr);

        var existingWorkout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutDto.Id && w.Owner == profile!.Id);
        if (existingWorkout is null)
            return new Result<WorkoutDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        // update existing workout fields
        existingWorkout.Name = workoutDto.Name;
        existingWorkout.Description = workoutDto.Description;
        existingWorkout.IsPublic = workoutDto.IsPublic;
        // existingWorkout.Exercises = workoutDto.Exercises.Select(e => e.ToWorkoutExerciseEntry());

        await _dataContext.SaveChangesAsync();

        return existingWorkout.ToWorkoutDto();
    }

    public async Task<Result<WorkoutDto>> DeleteWorkoutDto(Guid workoutId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDto>(profileErr);

        var workout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile!.Id);
        if (workout is null)
            return new Result<WorkoutDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout.ToWorkoutDto();
    }
}