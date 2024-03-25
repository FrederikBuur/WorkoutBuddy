using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public static class ExerciseDetailSeeder
{
    public static async Task SeedExerciseDetails(this DataContext context)
    {
        Console.WriteLine("Seeding Exercises");
        // exercise db:
        // https://www.jefit.com/exercises
        // https://exrx.net/Lists/Directory
        // https://www.bodybuilding.com/exercises/

        var json = await File.ReadAllTextAsync("Data/Seed/Json/exercises.json");
        IEnumerable<ExerciseDetail> initialExerciseDetails = JsonSerializer.Deserialize<IEnumerable<ExerciseDetail>>(json)
            ?? throw new Exception("Failed to deserialize exercieses.json");

        var creatorId = DatabaseHelper.CreatorId;
        var created = 0;
        var updated = 0;
        var skipped = 0;
        var failed = 0;
        var total = initialExerciseDetails.Count();

        foreach (var exerciseDetail in initialExerciseDetails)
        {
            try
            {
                exerciseDetail.Owner = creatorId;
                exerciseDetail.CreatorId = creatorId;
                exerciseDetail.IsPublic = true;

                var e = context.ExerciseDetails.SingleOrDefault(e => e.Id == exerciseDetail.Id);
                if (e is null)
                {
                    context.ExerciseDetails.Add(exerciseDetail);

                    Console.WriteLine($"Adding Exercise: {exerciseDetail.Name}");
                    created++;
                }
                else if (!e.Equals(exerciseDetail))
                {
                    context.Entry(e).CurrentValues.SetValues(exerciseDetail);
                    Console.WriteLine($"Update Exercise: {exerciseDetail.Name}");
                    updated++;
                }
                else
                {
                    skipped++;
                }
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed saving exercise: {exerciseDetail.Name}", e);
                failed++;
                throw;
            }
        }
        Console.WriteLine($"Result of {nameof(ExerciseDetailSeeder)}. Created: {created}, Updated: {updated}, Skipped: {skipped}, Failed: {failed}, Total: {total}");
    }
}
