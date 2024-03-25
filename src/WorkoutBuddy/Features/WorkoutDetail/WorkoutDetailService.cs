using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

public class WorkoutDetailService
{
    private readonly ILogger<WorkoutDetailService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;

    public WorkoutDetailService(ILogger<WorkoutDetailService> logger,
    DataContext dataContext,
    ProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<WorkoutDetail>>> SearchWorkoutDetails(
        VisibilityFilter visibilityFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<IEnumerable<WorkoutDetail>>(profileResult.Error!);

        #region predicates
        Expression<Func<WorkoutDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileResult.Value!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileResult.Value!.Id);

        Expression<Func<WorkoutDetail, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery)
            || e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var workouts = await _dataContext.WorkoutDetails
            .Where(visibilityPredicate)
            .Where(searchQueryPredicate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return workouts;
    }

    public async Task<Result<WorkoutDetail>> GetWorkoutDetailById(Guid workoutId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var workout = await _dataContext.WorkoutDetails
            .SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profileResult.Value!.Id);

        if (workout is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout could not be found")
            );

        if (workout?.IsPublic != true && workout?.Owner != profileResult.Value!.Id)
            return new Result<WorkoutDetail>(
                Error.Unauthorized("You dont have access to this workout")
            );

        return workout;
    }

    public async Task<Result<WorkoutDetail>> CreateWorkoutDetail(WorkoutDetail workout)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var w = workout;
        w.CreatorId = profileResult.Value?.Id ?? default;
        w.Owner = profileResult.Value?.Id ?? default;

        var result = _dataContext.Add(w);
        await _dataContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Result<WorkoutDetail>> UpdateWorkoutDetail(WorkoutDetail workout)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var existingWorkout = await _dataContext.WorkoutDetails.SingleOrDefaultAsync(w => w.Id == workout.Id && w.Owner == profileResult.Value!.Id);

        if (existingWorkout is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout could not be found")
            );

        if (existingWorkout.Owner != profileResult.Value!.Id)
            return new Result<WorkoutDetail>(
                Error.Unauthorized("You are not allowed to update this workout")
            );

        // update existing workout fields
        existingWorkout.Name = workout.Name;
        existingWorkout.Description = workout.Description;
        existingWorkout.IsPublic = workout.IsPublic;
        existingWorkout.Exercises = workout.Exercises;

        await _dataContext.SaveChangesAsync();

        return existingWorkout;
    }

    public async Task<Result<WorkoutDetail>> DeleteWorkoutDetail(Guid workoutId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var workout = await _dataContext.WorkoutDetails.SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profileResult.Value!.Id);
        if (workout is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout;
    }
}