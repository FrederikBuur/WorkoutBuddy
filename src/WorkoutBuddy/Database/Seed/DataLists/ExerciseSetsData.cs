
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Data.Seed;

public static class ExerciseSetsData
{
    public static readonly List<ExerciseSet> ExerciseSetsSeedData1 = new()
    {
        new(id: Guid.Parse("f5a4881d-4eab-48d4-be4c-4b66b5a0c202"),
            repetitions: 8,
            weight: 20,
            weightUnit: WeightUnit.Kilogram),
        new(id: Guid.Parse("a67edd57-bd16-4598-aa23-c2b5c7fe40c8"),
            repetitions: 8,
            weight: 30,
            weightUnit: WeightUnit.Kilogram),
        new(id: Guid.Parse("40f9ad02-a2a8-4a59-a76c-ecd906639057"),
            repetitions: 10,
            weight: 35,
            weightUnit: WeightUnit.Kilogram),
    };
    public static readonly List<ExerciseSet> ExerciseSetsSeedData2 = new()
    {
    new (id: Guid.Parse("6c8fe0a7-7b7a-4fd7-85b4-90f5f144e9e2"),
        repetitions: 12,
        weight: 25,
        weightUnit: WeightUnit.Kilogram),
    new (id: Guid.Parse("91d576d1-c48c-4463-b71e-d9a6f3f145db"),
        repetitions: 6,
        weight: 40,
        weightUnit: WeightUnit.Kilogram),
    new (id: Guid.Parse("37ac1f9e-7a5b-4e62-8a7b-2e59a4b9be10"),
        repetitions: 8,
        weight: 30,
        weightUnit: WeightUnit.Kilogram)
    };
    public static readonly List<ExerciseSet> ExerciseSetsSeedData3 = new()
    {
    new (id: Guid.Parse("d9c1577c-3c78-42f7-9d32-2a84c11b5c2e"),
        repetitions: 15,
        weight: 15,
        weightUnit: WeightUnit.Kilogram),
    new (id: Guid.Parse("0b4307b2-1ba4-4aa8-9ef3-9ed00995e1db"),
        repetitions: 8,
        weight: 35,
        weightUnit: WeightUnit.Kilogram),
    new (id: Guid.Parse("de56a4dc-2727-4fc1-b29f-16669d587e55"),
        repetitions: 10,
        weight: 40,
        weightUnit: WeightUnit.Kilogram)
    };
    public static readonly List<ExerciseSet> ExerciseSetsSeedData4 = new()
    {
    new (id: Guid.Parse("fe2cc8a7-1981-45a7-a8c8-d8c9bc2f7787"),
        repetitions: 10,
        weight: 20,
        weightUnit: WeightUnit.Kilogram),
    new (id: Guid.Parse("587f382a-28fb-45b7-83e1-8de60539afcd"),
        repetitions: 8,
        weight: 25,
        weightUnit: WeightUnit.Kilogram),
    new(id: Guid.Parse("1e63c24b-2c6b-456a-b35c-0d2933a894a9"),
        repetitions: 12,
        weight: 30,
        weightUnit: WeightUnit.Kilogram)
    };
    public static readonly List<ExerciseSet> ExerciseSetsSeedData5 = new()
    {
    new(id: Guid.Parse("7c8bd7a6-50d3-4c09-bfc0-ae6d9a3f20ab"),
        repetitions: 8,
        weight: 35,
        weightUnit: WeightUnit.Pound),
    new(id: Guid.Parse("29678947-2f2e-4a4f-a5cb-ecf68c015ab3"),
        repetitions: 10,
        weight: 40,
        weightUnit: WeightUnit.Pound),
    new(id: Guid.Parse("d1503d15-6f02-46a3-a4da-3e177f5ac751"),
        repetitions: 12,
        weight: 45,
        weightUnit: WeightUnit.Pound)
    };
}