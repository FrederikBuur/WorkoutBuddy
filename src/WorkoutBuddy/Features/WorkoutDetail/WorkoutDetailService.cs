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
    private readonly IProfileService _profileService;

    public WorkoutDetailService(ILogger<WorkoutDetailService> logger,
    DataContext dataContext,
    IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<WorkoutDetailDto>>> SearchWorkouts(
        VisibilityFilter visibilityFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<IEnumerable<WorkoutDetailDto>>(profileErr);


        #region predicates
        Expression<Func<WorkoutDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profile!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profile!.Id);

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
        return workouts.Select(w => w.ToWorkoutDetailDto()).ToList();
    }

    public async Task<Result<WorkoutDetailDto>> GetWorkoutDtoById(Guid workoutId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDetailDto>(profileErr);

        var workout = (await _dataContext.WorkoutDetails
            .SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile!.Id))
            ?.ToWorkoutDetailDto();

        if (workout is null)
            return new Result<WorkoutDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (workout?.IsPublic != true && workout?.Owner != profile!.Id)
            return new Result<WorkoutDetailDto>(
                new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have access to this workout")
            );

        return workout;
    }

    public async Task<Result<WorkoutDetailDto>> CreateWorkoutDto(WorkoutDetailDto workoutDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDetailDto>(profileErr);

        var w = workoutDto.ToWorkoutDetail();
        w.CreatorId = profile?.Id ?? default;
        w.Owner = profile?.Id ?? default;

        var result = _dataContext.Add(w);
        await _dataContext.SaveChangesAsync();
        return result.Entity.ToWorkoutDetailDto();
    }

    public async Task<Result<WorkoutDetailDto>> UpdateWorkoutDto(WorkoutDetailDto workoutDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDetailDto>(profileErr);

        var existingWorkout = await _dataContext.WorkoutDetails.SingleOrDefaultAsync(w => w.Id == workoutDto.Id && w.Owner == profile!.Id);

        if (existingWorkout is null)
            return new Result<WorkoutDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (existingWorkout.Owner != profile!.Id)
            return new Result<WorkoutDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "You are not allowed to update this workout")
            );

        // update existing workout fields
        existingWorkout.Name = workoutDto.Name;
        existingWorkout.Description = workoutDto.Description;
        existingWorkout.IsPublic = workoutDto.IsPublic;
        existingWorkout.Exercises = workoutDto.Exercises.Select(e => e.ToExerciseDetail()).ToList();

        await _dataContext.SaveChangesAsync();

        return existingWorkout.ToWorkoutDetailDto();
    }

    public async Task<Result<WorkoutDetailDto>> DeleteWorkoutDto(Guid workoutId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDetailDto>(profileErr);

        var workout = await _dataContext.WorkoutDetails.SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile!.Id);
        if (workout is null)
            return new Result<WorkoutDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout detail could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout.ToWorkoutDetailDto();
    }
}