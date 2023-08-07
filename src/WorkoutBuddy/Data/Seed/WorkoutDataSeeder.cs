using System.Text.Json;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public static class WorkoutDataSeeder
{
    public static async Task SeedWorkouts(this DataContext context)
    {
        Console.WriteLine("Seeding Workouts");

        var creatorId = DatabaseHelper.CreatorId;

        var json = await File.ReadAllTextAsync("Data/Seed/workouts.json");
        IEnumerable<Workout> initialWorkouts = JsonSerializer.Deserialize<IEnumerable<Workout>>(json)
            ?? throw new Exception("Failed to deserialize workouts.json");

        foreach (var workout in initialWorkouts)
        {
            workout.Owner = creatorId;
            workout.CreatorId = creatorId;
            workout.IsPublic = true;

            var w = context.Workouts.SingleOrDefault(w => w.Id == workout.Id);
            if (w is null)
            {
                context.Workouts.Add(workout);
                Console.WriteLine($"Adding Exercise: {workout.Name}");
            }
            else if (!w.Equals(workout))
            {
                context.Entry(w).CurrentValues.SetValues(workout);
                Console.WriteLine($"Update Exercise: {workout.Name}");
            }
            else
            {
                Console.WriteLine($"Skipped Exercise: {workout.Name}");
            }
        }
        await context.SaveChangesAsync();
    }
}