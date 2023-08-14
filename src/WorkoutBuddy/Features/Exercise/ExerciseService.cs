using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Controllers;
using WorkoutBuddy.Controllers.ExerciseModel;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features.ExerciseModel;

public class ExerciseService
{
    private readonly ILogger<ExerciseService> _logger;
    private readonly DataContext _dataContext;
    private readonly IProfileService _profileService;

    public ExerciseService(
        ILogger<ExerciseService> logger,
        DataContext dataContext,
        IProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<ExerciseDto>>> GetExercisesAsync(
        VisibilityFilter visibilityFilter,
        MuscleGroupType? muscleGroupType,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {

        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<IEnumerable<ExerciseDto>>(profileErr);


        #region predicates
        Expression<Func<Exercise, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profile!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profile!.Id);

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

        return exercises.Select(e => e.ToExerciseDto()).ToList();
    }

    public async Task<Result<ExerciseDto>> GetExerciseAsync(Guid exerciseId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDto>(profileErr);

        var exercise = (await _dataContext.Exercises
            .SingleOrDefaultAsync(e => e.Id == exerciseId && e.Owner == profile!.Id)
        )?.ToExerciseDto();

        if (exercise is null)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (exercise?.IsPublic != true && exercise?.Owner != profile!.Id)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have access to this workout")
            );

        return exercise;
    }

    public async Task<Result<ExerciseDto>> CreateExerciseAsync(ExerciseDto exerciseDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDto>(profileErr);

        var e = exerciseDto.ToExercise();
        e.CreatorId = profile!.Id;
        e.Owner = profile.Id;
        var result = _dataContext.Add(e);
        await _dataContext.SaveChangesAsync();

        return result.Entity.ToExerciseDto();
    }

    public async Task<Result<ExerciseDto>> UpdateExerciseAsync(ExerciseDto exercise)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDto>(profileErr);

        var existingExercise = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == exercise.Id);

        if (existingExercise is null)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (existingExercise.Owner != profile!.Id)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "You are not allowed to update this exercise")
            );

        existingExercise.IsPublic = exercise.IsPublic;
        existingExercise.Name = exercise.Name;
        existingExercise.Description = exercise.Description;
        existingExercise.ImageUrl = exercise.ImageUrl;
        existingExercise.MuscleGroups = exercise.MuscleGroups;

        await _dataContext.SaveChangesAsync();

        return existingExercise.ToExerciseDto();
    }

    public async Task<Result<ExerciseDto>> DeleteExerciseAsync(Guid exerciseId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDto>(profileErr);

        var exercise = await _dataContext.Exercises.SingleOrDefaultAsync(e => e.Id == exerciseId);
        if (exercise is null)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (exercise.Owner != profile!.Id)
            return new Result<ExerciseDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "You are not allowed to update this exercise")
            );

        var result = _dataContext.Remove(exercise);
        var deletedExercise = result.Entity.ToExerciseDto();
        await _dataContext.SaveChangesAsync();

        return deletedExercise;
    }

}