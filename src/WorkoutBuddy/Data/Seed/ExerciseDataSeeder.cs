using WorkoutBuddy.Controllers.Exercise.Model;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public static class ExerciseDataSeeder
{
    public static async Task SeedExercises(this DataContext context)
    {
        Console.WriteLine("Seeding Exercises");
        // exercise db:
        // https://www.jefit.com/exercises
        // https://exrx.net/Lists/Directory
        // https://www.bodybuilding.com/exercises/

        var creatorId = DatabaseHelper.CreatorId;

        var initialExercises = new List<ExerciseDto>() {
            new ExerciseDto()
            {
                Id = Guid.Parse("9f1a7d66-e070-424b-aa3b-dc1ff3cd3bff"),
                CreatorId = creatorId,
                Name = "Deadlift",
                Description = "The barbell deadlift is a classic bodybuilding exercise meant for putting on mass and building overall strength throughout the entire body.",
                PrimaryMuscleGroup = MuscleGroupType.UpperBack,
                SecondaryMuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Hamstring, MuscleGroupType.Glutes }
            },
            new ExerciseDto()
            {
                Id = Guid.Parse("8126c195-4f25-47a6-be90-9333ee061de6"),
                CreatorId = creatorId,
                Name = "Bench press",
                Description = "The bench press is a compound exercise that builds strength and muscle in the chest, triceps and shoulders. When many people think of listing, the bench press is often the first exercise that comes to mind",
                PrimaryMuscleGroup = MuscleGroupType.Chest,
                SecondaryMuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Tricep, MuscleGroupType.Shoulder }
            }
        };

        foreach (var exercise in initialExercises)
        {
            var e = context.Exercises.SingleOrDefault(e => e.Id == exercise.Id && e.CreatorId == exercise.CreatorId);
            if (e is null)
            {
                context.Exercises.Add(exercise);
                Console.WriteLine($"Adding Exercise: {exercise.Name}");
            }
            else if (!e.Equals(exercise))
            {
                context.Entry(e).CurrentValues.SetValues(exercise);
                Console.WriteLine($"Update Exercise: {exercise.Name}");
            }
            else
            {
                Console.WriteLine($"Skipped Exercise: {exercise.Name}");
            }
        }
        await context.SaveChangesAsync();
    }
}
