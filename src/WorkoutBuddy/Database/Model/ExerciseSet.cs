using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Data;

public class ExerciseSet : IEntityBase
{
    public int Repetitions { get; set; }
    public double Weight { get; set; }
    public WeightUnit WeightUnit { get; set; }
    public double OneRepMax { get; set; }

    // navigation properties
    public Guid ExerciseLogId { get; set; }
    public ExerciseLog? ExerciseLog { get; set; }

    // EF Core needs empty constructor
    public ExerciseSet() { }

    public ExerciseSet(
        Guid? id,
        int repetitions,
        double weight,
        WeightUnit weightUnit)
    {
        Id = id ?? Guid.NewGuid();
        Repetitions = repetitions;
        Weight = weight;
        WeightUnit = weightUnit;
        OneRepMax = weight / (1.0278 - 0.0278 * repetitions); // Matt Brzycki formular
    }
}
