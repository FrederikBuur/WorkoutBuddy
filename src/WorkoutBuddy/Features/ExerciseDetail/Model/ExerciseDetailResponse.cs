
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Features;

public record ExerciseDetailResponse(
    Guid Id,
    Guid Owner,
    Guid CreatorId,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsPublic,
    string MuscleGroups
)
{
    public ExerciseDetailResponse(ExerciseDetail ed) : this(
        ed.Id, ed.Owner, ed.CreatorId, ed.Name, ed.Description, ed.ImageUrl, ed.IsPublic, ed.MuscleGroups
    )
    { }
};
