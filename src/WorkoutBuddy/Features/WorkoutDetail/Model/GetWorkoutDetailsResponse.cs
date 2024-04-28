using Azure;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

public record GetWorkoutDetailsResponse(
    int totalPages,
    int CurrentPage,
    int TotalItems,
    bool lastPage,
    List<WorkoutDetailResponse> items
)
{
    public GetWorkoutDetailsResponse(Paginated<WorkoutDetail> p) : this(
        p.TotalPages,
        p.CurrentPage,
        p.TotalItems,
        p.LastPage,
        p.Items.Select(i => new WorkoutDetailResponse(i)).ToList()
    )
    { }
}