using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class Workout : IEntityBase
{
    public string Name { get; set; } = string.Empty;
    public DateTime LastPerformed { get; set; }
    public int Count { get; set; }

    // navigation properties
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
    public Guid WorkoutDetailId { get; set; }
    public WorkoutDetail? WorkoutDetail { get; set; }
    public ICollection<WorkoutLog> WorkoutLog { get; set; } = new List<WorkoutLog>();

    // EF Core needs empty constructor
    public Workout() { }

    public Workout(Guid? id, string name, DateTime lastPerformed, int count)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        LastPerformed = lastPerformed;
        Count = count;
    }
}
