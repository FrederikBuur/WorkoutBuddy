
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Util;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features;

public class SessionService
{
    private readonly ILogger<WorkoutDetailService> _logger;
    private readonly DataContext _dataContext;
    private readonly ProfileService _profileService;
    private readonly WorkoutService _workoutService;

    public SessionService(ILogger<WorkoutDetailService> logger,
        DataContext dataContext,
        ProfileService profileService,
        WorkoutService workoutService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _profileService = profileService;
        _workoutService = workoutService;
    }

    public async Task<Result<Paginated<WorkoutLog>>> GetWorkoutLogsForWorkoutPaginated(
        Guid workoutId,
        int pageNumber,
        int pageSize)
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<Paginated<WorkoutLog>>(profileResult.Error!);

        if (!await _workoutService.UserHasAccessToWorkout(workoutId))
            return new Result<Paginated<WorkoutLog>>(Error.Unauthorized("User does not have access to workout"));

        var filteredWorkoutLogs = _dataContext.WorkoutLog
            .Include(wl => wl.Workout)
            .Where(wl => wl.WorkoutId == workoutId)
            .OrderByDescending(wl => wl.CreatedAt);

        var workoutPage = await filteredWorkoutLogs.GetPage(
            pageNumber,
            pageSize
        );

        return workoutPage;
    }

    public async Task<Result<WorkoutLog>> GetWorkoutLogById(
        Guid workoutId
    )
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutLog>(profileResult.Error!);

        var workout = await _dataContext.WorkoutLog
            .Include(wl => wl.Workout)
            .Include(wl => wl.ExerciseLogs!)
            .ThenInclude(el => el.ExerciseSets)
            .SingleOrDefaultAsync(wl => wl.Id == workoutId
                && wl.Workout!.ProfileId == profileResult.Value.Id);

        if (workout is null)
        {
            return new Result<WorkoutLog>(Error.NotFound($"Workout with id: {workoutId} was not found"));
        }

        return workout;
    }

    public async Task<Result<WorkoutLog>> CreateWorkoutLog(
        WorkoutLogRequest workoutLogRequest
    )
    {
        var profileResult = _profileService.GetProfileResult();
        if (profileResult.IsFaulted)
            return new Result<WorkoutLog>(profileResult.Error!);

        var workout = await _dataContext.Workout
            .SingleOrDefaultAsync(w => w.Id == workoutLogRequest.WorkoutId
                && w.ProfileId == profileResult.Value.Id);

        if (workout is null)
        {
            return new Result<WorkoutLog>(Error.NotFound(
                $"Workout with id: {workoutLogRequest.WorkoutId} was not found"));
        }

        var workoutLog = new WorkoutLog(
              id: Guid.NewGuid(),
              completedAt: DateTime.UtcNow
        )
        {
            WorkoutId = workoutLogRequest.WorkoutId
        };

        var exerciseLogs = await CreateExerciseLogs(
            exerciseLogsRequests: workoutLogRequest.ExerciseLogsRequest,
            workoutLogId: workoutLog.Id,
            shouldSaveDbContext: false
        );

        workoutLog.ExerciseLogs = exerciseLogs.Value;

        var workoutLogEntity = await _dataContext.WorkoutLog.AddAsync(workoutLog);
        await _dataContext.SaveChangesAsync();

        return workoutLogEntity.Entity;
    }

    private async Task<Result<List<ExerciseLog>>> CreateExerciseLogs(
        List<ExerciseLogRequest> exerciseLogsRequests,
        Guid workoutLogId,
        bool shouldSaveDbContext
    )
    {
        var exerciseLogs = new List<ExerciseLog>();
        foreach (var exerciseLogsRequest in exerciseLogsRequests)
        {
            var exerciseLog = new ExerciseLog(
                id: Guid.NewGuid(),
                workoutLogId: workoutLogId,
                exerciseDetailId: exerciseLogsRequest.ExerciseDetailId
            );

            var exerciseSets = await CreateExerciseSets(
                exerciseSetRequests: exerciseLogsRequest.ExerciseSetsRequest,
                exerciseLogId: exerciseLog.Id,
                shouldSaveDbContext: false);

            exerciseLog.ExerciseSets = exerciseSets.Value;

            var exerciseLogEntity = await _dataContext.ExerciseLog.AddAsync(exerciseLog);
            exerciseLogs.Add(exerciseLogEntity.Entity);
        }

        if (shouldSaveDbContext)
            await _dataContext.SaveChangesAsync();

        return exerciseLogs;
    }

    private async Task<Result<List<ExerciseSet>>> CreateExerciseSets(
        List<ExerciseSetRequest> exerciseSetRequests,
        Guid exerciseLogId,
        bool shouldSaveDbContext
    )
    {
        var exerciseSets = new List<ExerciseSet>();
        foreach (var exerciseSetRequest in exerciseSetRequests)
        {
            var exerciseSet = new ExerciseSet(
                        id: Guid.NewGuid(),
                        repetitions: exerciseSetRequest.Repetitions,
                        weight: exerciseSetRequest.Weight,
                        weightUnit: exerciseSetRequest.WeightUnit
                    )
            {
                ExerciseLogId = exerciseLogId
            };

            var exerciseSetEntity = await _dataContext.ExerciseSet.AddAsync(exerciseSet);
            exerciseSets.Add(exerciseSetEntity.Entity);
        }

        if (shouldSaveDbContext)
            await _dataContext.SaveChangesAsync();

        return exerciseSets;
    }
}