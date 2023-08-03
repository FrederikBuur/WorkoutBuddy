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

    public async Task<IEnumerable<WorkoutDto>> SearchWorkouts(
        VisibilityFilter visibilityFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have a account");

        #region predicates
        Expression<Func<Workout, bool>> visibilityPredicate = (e) =>
            (visibilityFilter == VisibilityFilter.PUBLIC && e.IsPublic)
            || visibilityFilter == VisibilityFilter.OWNED && e.Owner == profile.Id
            || visibilityFilter == VisibilityFilter.ALL && (e.IsPublic || e.Owner == profile.Id);

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

        return workouts.Select(w => w.ToWorkoutDto());
    }

    public async Task<WorkoutDto> GetWorkoutDtoById(Guid workoutId)
    {
        var profile = _profileService.GetProfile();
        if (profile is null)
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have an account");

        var workout = (await _dataContext.Workouts
            .SingleOrDefaultAsync(w => w.Id == workoutId && w.Owner == profile.Id)
            )?.ToWorkoutDto();

        if (workout is null)
            throw new HttpResponseException(HttpStatusCode.NotFound, "Your workout could not be found");

        if (workout?.isPublic != true && workout?.owner != profile.Id)
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have access to this workout");

        return workout;
    }
}