using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util.ErrorHandling;
using WorkoutBuddy.Util;

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

    public async Task<Result<Paginated<ExerciseDetail>>> GetExercisesAsync(
        VisibilityFilter visibilityFilter,
        MuscleGroupType? muscleGroupType,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {

        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<Paginated<ExerciseDetail>>(profileResult.Error!);

        #region predicates
        Expression<Func<ExerciseDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic) ||
            visibilityFilter == VisibilityFilter.OWNED && e.Owner == profileResult.Value.Id ||
            visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profileResult.Value.Id);

        Expression<Func<ExerciseDetail, bool>> muscleGroupPredicate = (e) =>
            muscleGroupType == null ||
            e.MuscleGroups.ToLower().Contains(muscleGroupType!.ToString()!.ToLower() ?? "");

        Expression<Func<ExerciseDetail, bool>> searchQueryPredicate = (e) =>
            string.IsNullOrWhiteSpace(searchQuery) ||
            e.Name.ToLower().Contains(searchQuery!.ToLower() ?? "");

        #endregion

        var filteredExercises = _dataContext.ExerciseDetail
            .Where(visibilityPredicate)
            .Where(muscleGroupPredicate)
            .Where(searchQueryPredicate);

        var count = filteredExercises.Count();
        var totalPages = count / pageSize;
        if (count % pageSize != 0) totalPages++;

        var pagedExercises = await filteredExercises.Skip(Math.Max(0, pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Paginated<ExerciseDetail>(
            totalPages: totalPages,
            currentPage: pageNumber,
            pageSize: pageSize,
            totalItems: count,
            lastPage: totalPages == pageNumber,
            items: pagedExercises
        );
    }

    public async Task<Result<ExerciseDetail>> GetExerciseAsync(Guid exerciseId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var exercise = await _dataContext.ExerciseDetail
            .SingleOrDefaultAsync(e => e.Id == exerciseId && e.Owner == profileResult.Value.Id)
        ;

        if (exercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your exercise detail could not be found")
            );

        if (exercise?.IsPublic != true && exercise?.Owner != profileResult.Value.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You dont have access to this workout")
            );

        return exercise;
    }

    public async Task<Result<ExerciseDetail>> CreateExerciseAsync(CreateExerciseDetailRequest exerciseRequest)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var newExercise = new ExerciseDetail(
            null,
            exerciseRequest.owner,
            exerciseRequest.creatorId,
            exerciseRequest.name,
            exerciseRequest.description,
            exerciseRequest.imageUrl,
            exerciseRequest.isPublic,
            exerciseRequest.muscleGroup
        );

        var result = _dataContext.Add(newExercise);
        await _dataContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Result<ExerciseDetail>> UpdateExerciseAsync(UpdateExerciseDetailRequest exerciseRequest)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var existingExercise = await _dataContext.ExerciseDetail.SingleOrDefaultAsync(e => e.Id == exerciseRequest.id);

        if (existingExercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your exercise detail could not be found")
            );

        if (existingExercise.Owner != profileResult.Value.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You are not allowed to update this exercise")
            );

        existingExercise.IsPublic = exerciseRequest.isPublic;
        existingExercise.Name = exerciseRequest.name;
        existingExercise.Description = exerciseRequest.description;
        existingExercise.ImageUrl = exerciseRequest.imageUrl;
        existingExercise.MuscleGroups = exerciseRequest.muscleGroups;

        await _dataContext.SaveChangesAsync();

        return existingExercise;
    }

    public async Task<Result<ExerciseDetail>> DeleteExerciseAsync(Guid exerciseId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<ExerciseDetail>(profileResult.Error!);

        var exercise = await _dataContext.ExerciseDetail.SingleOrDefaultAsync(e => e.Id == exerciseId);
        if (exercise is null)
            return new Result<ExerciseDetail>(
                Error.NotFound("Your exercise detail could not be found")
            );

        if (exercise.Owner != profileResult.Value.Id)
            return new Result<ExerciseDetail>(
                Error.Unauthorized("You are not allowed to update this exercise")
            );

        var result = _dataContext.Remove(exercise);
        var deletedExercise = result.Entity;
        await _dataContext.SaveChangesAsync();

        return deletedExercise;
    }

}