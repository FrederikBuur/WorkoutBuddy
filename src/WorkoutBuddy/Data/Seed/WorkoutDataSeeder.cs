using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public static class WorkoutDataSeeder
{
    public static async Task SeedWorkouts(this DataContext context)
    {
        Console.WriteLine("Seeding Workouts");

        var creatorId = DatabaseHelper.CreatorId;

        var json = await File.ReadAllTextAsync("Data/Seed/workouts.json");
        ICollection<Workout> initialWorkouts = JsonSerializer.Deserialize<ICollection<Workout>>(json)
            ?? throw new Exception("Failed to deserialize workouts.json");

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var total = initialWorkouts.Count;

        foreach (var workout in initialWorkouts)
        {
            workout.Owner = creatorId;
            workout.CreatorId = creatorId;
            workout.IsPublic = true;

            var w = context.Workouts
                .SingleOrDefault(w => w.Id == workout.Id);

            if (w is null)
            {
                var test = context.Workouts.Add(workout);
                created++;
                Console.WriteLine($"Adding Workout: {workout.Name}");
            }
            else if (!w.Equals(workout))
            {
                context.Entry(w).CurrentValues.SetValues(workout);
                context.Entry(w).Collection(w => w.ExerciseEntries).CurrentValue = workout.ExerciseEntries;
                updated++;
                Console.WriteLine($"Update Workout: {workout.Name}");
            }
            else
            {
                skipped++;
                //Console.WriteLine($"Skipped Workout: {workout.Name}");
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine($"Result of {nameof(WorkoutDataSeeder)}. Created: {created}, Updated: {updated}, Skipped: {skipped}, Total: {total}");
    }
}