using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

public class WorkoutDetailService
{
    private readonly ILogger<WorkoutDetailService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;
    private readonly ExerciseDetailService _exerciseDetailService;

    public WorkoutDetailService(ILogger<WorkoutDetailService> logger,
    DataContext dataContext,
    ProfileService profileService,
    ExerciseDetailService exerciseDetailService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
        _exerciseDetailService = exerciseDetailService;
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
            .Include(wd => wd.Exercises)
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

    public async Task<Result<WorkoutDetail>> CreateWorkoutDetail(
        [FromBody] CreateWorkoutDetailRequest workoutRequest
    )
    {
        // checks that user exists
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        // creates workout detail
        var workoutDetail = new WorkoutDetail(
            null,
            profileResult.Value!.Id,
            profileResult.Value!.Id,
            workoutRequest.Name,
            workoutRequest.Description,
            workoutRequest.IsPublic
        );

        // adds existing exercise details to the new workout detial
        var exercises = new List<ExerciseDetail>();
        foreach (var exerciseId in workoutRequest.ExerciseIds)
        {
            var exercise = await _dataContext.ExerciseDetails.FindAsync(exerciseId);

            if (exercise is not null)
                exercises.Add(exercise);
            else
                Console.WriteLine($"exercise id: '{exerciseId}' do not exist");
        }

        // saves data to db
        workoutDetail.Exercises = exercises;
        var result = _dataContext.WorkoutDetails.Add(workoutDetail);
        await _dataContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Result<WorkoutDetail>> UpdateWorkoutDetail(
        [FromBody] UpdateWorkoutDetailRequest workoutRequest
    )
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var existingWorkoutDetail = await _dataContext.WorkoutDetails.SingleOrDefaultAsync(w =>
            w.Id == workoutRequest.Id
            && w.Owner == workoutRequest.Owner
            && w.Owner == profileResult.Value!.Id
        );

        if (existingWorkoutDetail is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout could not be found")
            );

        existingWorkoutDetail.Name = workoutRequest.Name;
        existingWorkoutDetail.Description = workoutRequest.Description;
        existingWorkoutDetail.IsPublic = workoutRequest.IsPublic;

        await _dataContext.SaveChangesAsync();

        return existingWorkoutDetail;
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