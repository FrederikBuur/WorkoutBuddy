using Azure;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features;

public record GetExercisesResponse(
    int totalPages,
    int CurrentPage,
    int TotalItems,
    bool lastPage,
    List<ExerciseDetailResponse> items
)
{
    public GetExercisesResponse(Paginated<ExerciseDetail> p) : this(
        p.TotalPages,
        p.CurrentPage,
        p.TotalItems,
        p.LastPage,
        p.Items.Select(i => new ExerciseDetailResponse(i)).ToList()
    )
    { }
}