using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

public record WorkoutDetailResponse(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    bool IsPublic,
    IEnumerable<ExerciseDetailResponse>? Exercises
)
{
    public WorkoutDetailResponse(WorkoutDetail wd) : this(
        wd.Id,
        wd.Owner,
        wd.CreatorId,
        wd.Name,
        wd.Description,
        wd.IsPublic,
        wd.Exercises?.Select(e => new ExerciseDetailResponse(e))
    )
    { }
};