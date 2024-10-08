
using WorkoutBuddy.Data.Seed;

namespace WorkoutBuddy.Data;

internal static class DatabaseHelper
{
    public static readonly Guid CreatorId = Guid.Parse("ea21cae2-728c-4679-a14f-988e1ccfcd64");

    internal static async Task SeedDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // seed database
        Console.WriteLine("Begin seeding");
        await context.SeedProfiles();
        await context.SeedExerciseDetails();
        await context.SeedWorkoutDetails();
        await context.SeedWorkoutAndSessionDataSeeder();//scope);
        Console.WriteLine("Finished seeding");

        scope.Dispose();
    }
}