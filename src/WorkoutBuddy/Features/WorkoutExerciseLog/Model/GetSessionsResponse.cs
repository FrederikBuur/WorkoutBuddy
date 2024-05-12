using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

// todo + easy constructor, see GetWorkoutDetailsResponse
public record GetSessionsResponse(
    Guid WorkoutId,
    int TotalPages,
    int CurrentPage,
    int TotalItems,
    bool LastPage,
    List<WorkoutLogResponse> Items
)
{
    public GetSessionsResponse(Guid WorkoutId, Paginated<WorkoutLog> p) : this(
        WorkoutId,
        p.TotalPages,
        p.CurrentPage,
        p.TotalItems,
        p.LastPage,
        p.Items.Select(i => new WorkoutLogResponse(i)).ToList()
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
        wl.ExerciseLogs?.Select(el => new ExerciseLogResponse(el))?.ToList()
    )
    { }
};

public record ExerciseLogResponse(
    Guid Id,
    Guid ExerciseDetailId,
    List<ExerciseSetResponse>? ExerciseSets
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