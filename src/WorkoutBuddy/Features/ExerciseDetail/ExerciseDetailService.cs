using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

public class ExerciseDetailService
{
    private readonly ILogger<ExerciseDetailService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;

    public ExerciseDetailService(
        ILogger<ExerciseDetailService> logger,
        DataContext dataContext,
        ProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<ExerciseDetail>>> GetExercisesAsync(
        VisibilityFilter visibilityFilter,
        MuscleGroupType? muscleGroupType,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {

        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<IEnumerable<ExerciseDetail>>(profileResult.Error!);

        #region predicates
        Expression<Func<ExerciseDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileResult.Value!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileResult.Value!.Id);

        Expression<Func<ExerciseDetail, bool>> muscleGroupPredicate = (e) =>
            muscleGroupType != null
                &&
                e.MuscleGroups.ToLower().Contains(muscleGroupType!.ToString()!.ToLower() ?? "");

        Expression<Func<ExerciseDetail, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery)
            || e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var exercises = await _dataContext.ExerciseDetails
            .Where(visibilityPredicate)
            .Where(muscleGroupPredicate)
            .Where(searchQueryPredicate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return exercises;
    }

    public async Task<Result<ExerciseDetail>> GetExerciseAsync(Guid exerciseId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var exercise = await _dataContext.ExerciseDetails
            .SingleOrDefaultAsync(e => e.Id == exerciseId && e.Owner == profileResult.Value!.Id)
        ;

        if (exercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your workout could not be found")
            );

        if (exercise?.IsPublic != true && exercise?.Owner != profileResult.Value!.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You dont have access to this workout")
            );

        return exercise;
    }

    public async Task<Result<ExerciseDetail>> CreateExerciseAsync(ExerciseDetail exercise)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var result = _dataContext.Add(exercise);
        await _dataContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Result<ExerciseDetail>> UpdateExerciseAsync(ExerciseDetail exercise)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var existingExercise = await _dataContext.ExerciseDetails.SingleOrDefaultAsync(e => e.Id == exercise.Id);

        if (existingExercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your workout could not be found")
            );

        if (existingExercise.Owner != profileResult.Value!.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You are not allowed to update this exercise")
            );

        existingExercise.IsPublic = exercise.IsPublic;
        existingExercise.Name = exercise.Name;
        existingExercise.Description = exercise.Description;
        existingExercise.ImageUrl = exercise.ImageUrl;
        existingExercise.MuscleGroups = exercise.MuscleGroups;

        await _dataContext.SaveChangesAsync();

        return existingExercise;
    }

    public async Task<Result<ExerciseDetail>> DeleteExerciseAsync(Guid exerciseId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var exercise = await _dataContext.ExerciseDetails.SingleOrDefaultAsync(e => e.Id == exerciseId);
        if (exercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your workout could not be found")
            );

        if (exercise.Owner != profileResult.Value!.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You are not allowed to update this exercise")
            );

        var result = _dataContext.Remove(exercise);
        var deletedExercise = result.Entity;
        await _dataContext.SaveChangesAsync();

        return deletedExercise;
    }

}