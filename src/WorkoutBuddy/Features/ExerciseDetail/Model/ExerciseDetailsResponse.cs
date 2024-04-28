using Azure;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

public record ExerciseDetailsResponse(
    int TotalPages,
    int CurrentPage,
    int PageSize,
    int TotalItems,
    bool LastPage,
    List<ExerciseDetailResponse> items
)
{
    public ExerciseDetailsResponse(Paginated<ExerciseDetail> p) : this(
        p.TotalPages,
        p.CurrentPage,
        p.PageSize,
        p.TotalItems,
        p.LastPage,
        p.Items.Select(i => new ExerciseDetailResponse(i)).ToList()
    )
    { }
}