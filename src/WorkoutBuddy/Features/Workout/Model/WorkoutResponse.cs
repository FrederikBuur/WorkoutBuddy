
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