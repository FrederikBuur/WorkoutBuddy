using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features;

public class WorkoutDetailsService
{
    private readonly ILogger<WorkoutDetailsService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfilesService _profileService;

    public WorkoutDetailsService(ILogger<WorkoutDetailsService> logger,
    DataContext dataContext,
    ProfilesService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<Paginated<WorkoutDetail>>> SearchWorkoutDetails(
        VisibilityFilter visibilityFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<Paginated<WorkoutDetail>>(profileResult.Error!);

        #region predicates
        Expression<Func<WorkoutDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic) ||
            visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileResult.Value.Id ||
            visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileResult.Value.Id);

        Expression<Func<WorkoutDetail, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery) ||
            e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var filteredWorkoutDetails = _dataContext.WorkoutDetail
            .Include(wd => wd.Exercises)
            .Where(visibilityPredicate)
            .Where(searchQueryPredicate);

        var count = filteredWorkoutDetails.Count();
        var totalPages = count / pageSize;
        if (count % pageSize != 0) totalPages++;

        var pagedWorkoutDetails = await filteredWorkoutDetails.Skip(Math.Max(0, pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Paginated<WorkoutDetail>(
            totalPages: totalPages,
            currentPage: pageNumber,
            pageSize: pageSize,
            totalItems: count,
            lastPage: totalPages == pageNumber,
            items: pagedWorkoutDetails
        );
    }

    public async Task<Result<WorkoutDetail>> GetWorkoutDetailById(Guid workoutId)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var workout = await _dataContext.WorkoutDetail
            .SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profileResult.Value.Id);

        if (workout is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout detail could not be found")
            );

        if (workout?.IsPublic != true && workout?.Owner != profileResult.Value.Id)
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
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        // creates workout detail
        var workoutDetail = new WorkoutDetail(
            null,
            profileResult.Value.Id,
            profileResult.Value.Id,
            workoutRequest.Name,
            workoutRequest.Description,
            workoutRequest.IsPublic
        );

        // adds existing exercise details to the new workout detial
        var exercises = new List<ExerciseDetail>();
        foreach (var exerciseId in workoutRequest.ExerciseIds)
        {
            var exercise = await _dataContext.ExerciseDetail.FindAsync(exerciseId);

            if (exercise is not null)
                exercises.Add(exercise);
            else
                Console.WriteLine($"exercise id: '{exerciseId}' do not exist");
        }

        // saves data to db
        workoutDetail.Exercises = exercises;
        var result = _dataContext.WorkoutDetail.Add(workoutDetail);
        await _dataContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Result<WorkoutDetail>> UpdateWorkoutDetail(
        [FromBody] UpdateWorkoutDetailRequest workoutRequest
    )
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var existingWorkoutDetail = await _dataContext.WorkoutDetail.SingleOrDefaultAsync(w =>
            w.Id == workoutRequest.Id
            && w.Owner == workoutRequest.Owner
            && w.Owner == profileResult.Value.Id
        );

        if (existingWorkoutDetail is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout detail could not be found")
            );

        existingWorkoutDetail.Name = workoutRequest.Name;
        existingWorkoutDetail.Description = workoutRequest.Description;
        existingWorkoutDetail.IsPublic = workoutRequest.IsPublic;

        await _dataContext.SaveChangesAsync();

        return existingWorkoutDetail;
    }

    public async Task<Result<WorkoutDetail>> DeleteWorkoutDetail(Guid workoutId)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<WorkoutDetail>(profileResult.Error!);

        var workout = await _dataContext.WorkoutDetail.SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profileResult.Value.Id);
        if (workout is null)
            return new Result<WorkoutDetail>(
                Error.NotFound("Your workout detail could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout;
    }
}