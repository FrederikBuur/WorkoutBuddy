
namespace WorkoutBuddy.Features;

public record WorkoutResponse(
    Guid Id,
    string Name,
    DateTime? LastPerformed,
    int Count,
    WorkoutDetailResponse? WorkoutDetail,
    List<WorkoutLogResponse>? WorkoutLogs
)
{
    public WorkoutResponse(Workout w) : this(
        w.Id,
        w.Name,
        w.LastPerformed,
        w.Count,
        w.WorkoutDetail is not null ? new WorkoutDetailResponse(w.WorkoutDetail) : null,
        w.WorkoutLogs?.Select(wl => new WorkoutLogResponse(wl)).ToList()
    )
    { }
};

public record WorkoutLogResponse(
    Guid Id,
    DateTime CompletedAt,
    List<ExerciseLogResponse>? ExerciseLogs
)
{
    public WorkoutLogResponse(WorkoutLog wl) : this(
        wl.Id,
        wl.CompletedAt,
        wl.ExerciseLog?.Select(el => new ExerciseLogResponse(el))?.ToList()
    )
    { }
};

public record ExerciseLogResponse(
    Guid Id,
    Guid ExerciseDetailId,
    List<ExerciseSetResponse>? exerciseSets
)
{
    public ExerciseLogResponse(ExerciseLog el) : this(
        el.Id,
        el.ExerciseDetailId,
        el.ExerciseSets?.Select(es => new ExerciseSetResponse(es))?.ToList()
    )
    { }
};

public record ExerciseSetResponse(
    Guid Id,
    int Repetitions,
    double Weight,
    WeightUnit WeightUnit,
    double OneRepMax
)
{
    public ExerciseSetResponse(ExerciseSet es) : this(
        es.Id,
        es.Repetitions,
        es.Weight,
        es.WeightUnit,
        es.OneRepMax
    )
    { }
};