using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features;

public class WorkoutsService
{
    private readonly ILogger<WorkoutsService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfilesService _profileService;
    private readonly WorkoutDetailsService _workoutDetailService;

    public WorkoutsService(ILogger<WorkoutsService> logger,
    DataContext dataContext,
    ProfilesService profileService,
    WorkoutDetailsService workoutDetailService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
        _workoutDetailService = workoutDetailService;
    }

    public async Task<Result<IEnumerable<Workout>>> GetWorkoutsForProfile()
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<IEnumerable<Workout>>(profileResult.Error!);

        var workouts = await _dataContext.Workout
            .Where(w => w.ProfileId == profileResult.Value.Id)
            .ToListAsync();

        return workouts;
    }


    public async Task<Result<Workout>> GetWorkoutById(Guid id)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<Workout>(profileResult.Error!);

        var workout = await _dataContext.Workout
            .Include(w => w.WorkoutDetail!)
            .Include(w => w.WorkoutLogs!)
            .ThenInclude(wl => wl.ExerciseLogs!)
            .ThenInclude(el => el.ExerciseSets!)
            .SingleOrDefaultAsync(w => w.ProfileId == profileResult.Value.Id && w.Id == id);

        if (workout is null)
            return new Result<Workout>(Error.NotFound("Your workout could not be found"));

        return workout;
    }

    public async Task<Result<Workout>> CreateWorkout(CreateWorkoutRequest createWorkoutRequest)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<Workout>(profileResult.Error);

        var workoutDetailResult = await _workoutDetailService.GetWorkoutDetailById(createWorkoutRequest.WorkoutDetailId);
        if (workoutDetailResult.IsFaulted)
        {
            return new Result<Workout>(workoutDetailResult.Error);
        }

        if (workoutDetailResult.Value.Owner != profileResult.Value.Id
            && !workoutDetailResult.Value.IsPublic)
        {
            return new Result<Workout>(Error.BadRequest(
                $"Workout detail: {createWorkoutRequest.WorkoutDetailId}, could not be found or missing access"));
        }

        var workout = new Workout(
            id: Guid.NewGuid(),
            name: createWorkoutRequest.Name,
            lastPerformed: null,
            count: 0
        )
        {
            Profile = profileResult.Value,
            WorkoutDetail = workoutDetailResult.Value
        };

        var createdWorkout = await _dataContext.Workout.AddAsync(workout);
        await _dataContext.SaveChangesAsync();

        if (createdWorkout?.Entity is not null)
        {
            return new Result<Workout>(createdWorkout.Entity);
        }
        else
        {
            return new Result<Workout>(Error.InternalServerError());
        }
    }

    public async Task<Result<Workout>> DeleteWorkout(Guid workoutId)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return new Result<Workout>(profileResult.Error!);

        var workout = await _dataContext.Workout.SingleOrDefaultAsync(w => w.Id == workoutId && w.ProfileId == profileResult.Value.Id);
        if (workout is null)
            return new Result<Workout>(
                Error.NotFound("Your workout detail could not be found")
            );

        var res = _dataContext.Remove(workout);
        await _dataContext.SaveChangesAsync();

        return res.Entity;
    }

    public async Task<bool> UserHasAccessToWorkout(Guid workoutId, Guid? userId = null)
    {
        var profileResult = _profileService.GetProfile();
        if (profileResult.IsFaulted)
            return false;

        var idToCheck = userId ?? profileResult.Value.Id;

        var workout = await _dataContext.Workout
            .SingleOrDefaultAsync(w => w.Id == workoutId &&
                w.ProfileId == profileResult.Value.Id);

        return workout is not null;
    }
}