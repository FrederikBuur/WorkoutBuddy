namespace WorkoutBuddy.Features;

public record WorkoutResponse(
    Guid id,
    string name,
    DateTime lastPerformed,
    int count
)
{
    public WorkoutResponse(Workout w) : this(
        w.Id, w.Name, w.LastPerformed, w.Count
    )
    { }
};