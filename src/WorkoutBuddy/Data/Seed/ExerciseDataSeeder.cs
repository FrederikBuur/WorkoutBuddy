using System.Text.Json;
using WorkoutBuddy.Controllers.ExerciseModel;
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

        var json = await File.ReadAllTextAsync("Data/Seed/exercises.json");
        ICollection<Exercise> initialExercises = JsonSerializer.Deserialize<ICollection<Exercise>>(json) ?? throw new Exception("Failed to deserialize exercieses.json");

        foreach (var exercise in initialExercises)
        {
            exercise.Owner = creatorId;
            exercise.CreatorId = creatorId;
            exercise.IsPublic = true;

            var e = context.Exercises.SingleOrDefault(e => e.Id == exercise.Id);
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
