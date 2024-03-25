using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

public class WorkoutService
{
    private readonly ILogger<WorkoutService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;

    public WorkoutService(ILogger<WorkoutService> logger,
    DataContext dataContext,
    ProfileService profileService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
    }

    public async Task<Result<IEnumerable<Workout>>> GetWorkoutsForProfile()
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<IEnumerable<Workout>>(profileResult.Error!);

        var workouts = await _dataContext.Workouts
            .Where(w => w.ProfileId == profileResult.Value!.Id)
            .ToListAsync();
        return workouts.Select(w => w).ToList();
    }

    public async Task<Result<Workout>> GetWorkoutById(Guid id)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<Workout>(profileResult.Error!);


        var workout = await _dataContext.Workouts
            .SingleOrDefaultAsync(w => w.ProfileId == profileResult.Value!.Id && w.Id == id);

        if (workout is null)
            return new Result<Workout>(
                Error.NotFound("Your workout could not be found")
                );

        return workout;
    }

    public async Task<Result<Workout>> DeleteWorkout(Guid workoutId)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<Workout>(profileResult.Error!);

        var workout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutId && w.ProfileId == profileResult.Value!.Id);
        if (workout is null)
            return new Result<Workout>(
                Error.NotFound("Your workout detail could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout;
    }
}