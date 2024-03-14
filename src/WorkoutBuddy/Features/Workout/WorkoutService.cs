using System.Net;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

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

    public async Task<Result<IEnumerable<WorkoutDto>>> GetWorkoutsForProfile()
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<IEnumerable<WorkoutDto>>(profileErr);

        var workouts = await _dataContext.Workouts
            .Where(w => w.ProfileId == profile!.Id)
            .ToListAsync();
        return workouts.Select(w => w.ToWorkoutDto()).ToList();
    }

    public async Task<Result<WorkoutDto>> DeleteWorkout(Guid workoutId)
    {
        var profileErr = _profileService.ProfileMissingAsException(out var profile);
        if (profileErr is not null)
            return new Result<WorkoutDto>(profileErr);

        var workout = await _dataContext.Workouts.SingleOrDefaultAsync(w => w.Id == workoutId && w.ProfileId == profile!.Id);
        if (workout is null)
            return new Result<WorkoutDto>(
                new HttpResponseException(HttpStatusCode.NotFound, "Your workout detail could not be found")
            );

        _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();
        return workout.ToWorkoutDto();
    }
}