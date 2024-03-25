using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public class Workout : IEntityBase
{
    public string Name { get; set; }
    public DateTime LastPerformed { get; set; }
    public int Count { get; set; }

    // navigation properties
    public Guid ProfileId { get; set; }
    public Profile? Profile { get; set; }
    public Guid WorkoutDetailId { get; set; }
    public WorkoutDetail? WorkoutDetail { get; set; }
    public ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();

    public Workout(Guid? id, string name, DateTime lastPerformed, int count)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        LastPerformed = lastPerformed;
        Count = count;
    }
}

public class WorkoutSet : IEntityBase
{
    public DateTime CompletedAt { get; set; }

    // navigation property
    public Guid WorkoutId { get; set; }
    public Workout? Workout { get; set; }
    public ICollection<ExerciseSet> ExerciseSets { get; set; } = new List<ExerciseSet>();
}

public class ExerciseSet : IEntityBase
{
    public DateTime CompletedAt { get; set; }
    public int Repetitions { get; set; }
    public int Weight { get; set; }
    public WeightUnit WeightUnit { get; set; }
    public int OneRepMax { get; set; }

    // navigation properties
    public Guid WorkoudSetId { get; set; }
    public WorkoutSet? WorkoutSet { get; set; }
    public Guid ExerciseDetailId { get; set; }
    public ExerciseDetail? ExerciseDetail { get; set; }
}

public enum WeightUnit
{
    Kilogram,
    Pound
}