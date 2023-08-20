using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

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

    public async Task<Result<IEnumerable<ExerciseDetailDto>>> GetExercisesAsync(
        VisibilityFilter visibilityFilter,
        MuscleGroupType? muscleGroupType,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {

        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<IEnumerable<ExerciseDetailDto>>(profileErr);


        #region predicates
        Expression<Func<ExerciseDetail, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profile!.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profile!.Id);

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

        return exercises.Select(e => e.ToExerciseDetailDto()).ToList();
    }

    public async Task<Result<ExerciseDetailDto>> GetExerciseAsync(Guid exerciseId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDetailDto>(profileErr);

        var exercise = (await _dataContext.ExerciseDetails
            .SingleOrDefaultAsync(e => e.Id == exerciseId && e.Owner == profile!.Id)
        )?.ToExerciseDetailDto();

        if (exercise is null)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (exercise?.IsPublic != true && exercise?.Owner != profile!.Id)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have access to this workout")
            );

        return exercise;
    }

    public async Task<Result<ExerciseDetailDto>> CreateExerciseAsync(ExerciseDetailDto exerciseDto)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDetailDto>(profileErr);

        var e = exerciseDto.ToExerciseDetail();
        e.CreatorId = profile!.Id;
        e.Owner = profile.Id;
        var result = _dataContext.Add(e);
        await _dataContext.SaveChangesAsync();

        return result.Entity.ToExerciseDetailDto();
    }

    public async Task<Result<ExerciseDetailDto>> UpdateExerciseAsync(ExerciseDetailDto exercise)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDetailDto>(profileErr);

        var existingExercise = await _dataContext.ExerciseDetails.SingleOrDefaultAsync(e => e.Id == exercise.Id);

        if (existingExercise is null)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (existingExercise.Owner != profile!.Id)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "You are not allowed to update this exercise")
            );

        existingExercise.IsPublic = exercise.IsPublic;
        existingExercise.Name = exercise.Name;
        existingExercise.Description = exercise.Description;
        existingExercise.ImageUrl = exercise.ImageUrl;
        existingExercise.MuscleGroups = exercise.MuscleGroups;

        await _dataContext.SaveChangesAsync();

        return existingExercise.ToExerciseDetailDto();
    }

    public async Task<Result<ExerciseDetailDto>> DeleteExerciseAsync(Guid exerciseId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<ExerciseDetailDto>(profileErr);

        var exercise = await _dataContext.ExerciseDetails.SingleOrDefaultAsync(e => e.Id == exerciseId);
        if (exercise is null)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found")
            );

        if (exercise.Owner != profile!.Id)
            return new Result<ExerciseDetailDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "You are not allowed to update this exercise")
            );

        var result = _dataContext.Remove(exercise);
        var deletedExercise = result.Entity.ToExerciseDetailDto();
        await _dataContext.SaveChangesAsync();

        return deletedExercise;
    }

}