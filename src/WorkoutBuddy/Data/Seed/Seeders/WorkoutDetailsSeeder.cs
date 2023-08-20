using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data;

public static class WorkoutDetailsSeeder
{
    public static async Task SeedWorkoutDetails(this DataContext context)
    {
        Console.WriteLine("Seeding Workouts");

        var json = await File.ReadAllTextAsync("Data/Seed/Json/workouts.json");
        IEnumerable<WorkoutDetail> seedingWorkoutDetails = JsonSerializer.Deserialize<IEnumerable<WorkoutDetail>>(json)
            ?? throw new Exception("Failed to deserialize workouts.json");

        var creatorId = DatabaseHelper.CreatorId;
        var created = 0;
        var updated = 0;
        var skipped = 0;
        var failed = 0;
        var total = seedingWorkoutDetails.Count();

        foreach (var seedingWorkout in seedingWorkoutDetails)
        {
            try
            {
                seedingWorkout.Owner = creatorId;
                seedingWorkout.CreatorId = creatorId;
                seedingWorkout.IsPublic = true;

                var existingWorkout = context.WorkoutDetails
                    .Include(w => w.Exercises)
                    .SingleOrDefault(w => w.Id == seedingWorkout.Id);

                if (existingWorkout is null)
                {
                    created = await CreateWorkoutAndRelations(context, created, seedingWorkout);
                }
                else if (!existingWorkout.Equals(seedingWorkout)
                    || !Enumerable.SequenceEqual(existingWorkout.Exercises.Select(w => w.Id), seedingWorkout.Exercises.Select(w => w.Id)))
                {
                    updated = await UpdateWorkoutAndRelations(context, updated, seedingWorkout, existingWorkout);
                }
                else
                {
                    skipped++;
                }
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to saiving workout: {seedingWorkout.Name}", e);
                failed++;
                throw;
            }
        }
        Console.WriteLine($"Result of {nameof(WorkoutDetailsSeeder)}. Created: {created}, Updated: {updated}, Skipped: {skipped}, Failed: {failed}, Total: {total}");
    }

    private static async Task<int> CreateWorkoutAndRelations(DataContext context, int created, WorkoutDetail seedingWorkout)
    {
        // add relation between workout detail and exercise detail
        var updatedExercises = new List<ExerciseDetail>();
        foreach (var ed in seedingWorkout.Exercises)
        {
            var existingExercise = await context.ExerciseDetails.FindAsync(ed.Id);
            if (existingExercise is not null) updatedExercises.Add(existingExercise);
            else Console.WriteLine($"Could not add exercise id '{ed.Id}' to workout {seedingWorkout.Name} since exercise with that id does not exist");
        }
        seedingWorkout.Exercises = updatedExercises;

        // add workout detial
        context.WorkoutDetails.Add(seedingWorkout);

        Console.WriteLine($"Adding Workout: {seedingWorkout.Name}");
        return created++;
    }

    private static async Task<int> UpdateWorkoutAndRelations(DataContext context, int updated, WorkoutDetail seedingWorkout, WorkoutDetail existingWorkout)
    {
        if (!existingWorkout.Equals(seedingWorkout))
        {
            context.Entry(existingWorkout).CurrentValues.SetValues(seedingWorkout);
        }

        if (!Enumerable.SequenceEqual(existingWorkout.Exercises.Select(w => w.Id), seedingWorkout.Exercises.Select(w => w.Id)))
        {
            var updatedExercises = new List<ExerciseDetail>();
            foreach (var ed in seedingWorkout.Exercises)
            {
                var existingExercise = await context.ExerciseDetails.FindAsync(ed.Id);
                if (existingExercise is not null) updatedExercises.Add(existingExercise);
                else Console.WriteLine($"Could not add exercise id '{ed.Id}' to workout {seedingWorkout.Name} since exercise with that id does not exist");
            }
            existingWorkout.Exercises = updatedExercises;
        }

        context.WorkoutDetails.Update(existingWorkout);
        Console.WriteLine($"Update Workout: {existingWorkout.Name}");
        return updated++;
    }
}