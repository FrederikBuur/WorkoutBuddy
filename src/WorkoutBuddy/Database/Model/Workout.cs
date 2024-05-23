using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class Workout : IEntityBase
{
    public string Name { get; set; } = string.Empty;
    public DateTime? LastPerformed { get; set; }
    public int Count { get; set; }

    // navigation properties
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
    public Guid WorkoutDetailId { get; set; }
    public WorkoutDetail? WorkoutDetail { get; set; }
    public ICollection<WorkoutLog>? WorkoutLogs { get; set; }

    // EF Core needs empty constructor
    public Workout() { }

    public Workout(Guid? id, string name, DateTime? lastPerformed, int count)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        LastPerformed = lastPerformed;
        Count = count;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not Workout other) return false;

        return Id == other.Id
        && Name == other.Name
        && LastPerformed == other.LastPerformed
        && Count == other.Count;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(Name);
        hash.Add(LastPerformed);
        hash.Add(Count);
        hash.Add(WorkoutLogs);
        return hash.ToHashCode();
    }
}
