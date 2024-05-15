using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Data.Seed;

public static class WorkoutAndSessionDataSeeder
{
    public static async Task SeedWorkoutAndSessionDataSeeder(this DataContext context)//, IServiceScope scope)
    {
        // var workoutService = scope.ServiceProvider.GetService<WorkoutService>();
        // var sessionService = scope.ServiceProvider.GetService<SessionService>();

        var workouts = WorkoutsData.WorkoutsSeedData1;

        var workoutCreated = 0;
        var workoutUpdated = 0;
        var workoutSkipped = 0;
        var workoutTotal = 0;

        var workoutLogsCreated = 0;
        var workoutLogsUpdated = 0;
        var workoutLogsSkipped = 0;
        var workoutLogsTotal = 0;

        var exerciseLogsCreated = 0;
        var exerciseLogsUpdated = 0;
        var exerciseLogsSkipped = 0;
        var exerciseLogsTotal = 0;

        var exerciseSetsCreated = 0;
        var exerciseSetsUpdated = 0;
        var exerciseSetsSkipped = 0;
        var exerciseSetsTotal = 0;

        foreach (var workout in workouts)
        {
            if (workout.WorkoutLogs is null ||
                !workout.WorkoutLogs.Any())
                Console.WriteLine("Workout logs where null or empty");
            foreach (var workoutLog in workout.WorkoutLogs!)
            {
                if (workoutLog.ExerciseLogs is null ||
                    !workoutLog.ExerciseLogs.Any())
                    Console.WriteLine("Exercise logs where null or empty");
                foreach (var exerciseLog in workoutLog.ExerciseLogs!)
                {
                    if (exerciseLog.ExerciseSets is null ||
                    !exerciseLog.ExerciseSets.Any())
                        Console.WriteLine("Exercise sets where null or empty");

                    var exerciseSetsStatus = await SeedExerciseSets(context, exerciseLog);
                    exerciseSetsCreated += exerciseSetsStatus.exerciseSetsCreated;
                    exerciseSetsUpdated += exerciseSetsStatus.exerciseSetsUpdated;
                    exerciseSetsSkipped += exerciseSetsStatus.exerciseSetsSkipped;
                    exerciseSetsTotal += exerciseSetsStatus.exerciseSetsTotal;

                    var el = await context.ExerciseLog.SingleOrDefaultAsync(x => x.Id == exerciseLog.Id);
                    if (el is null)
                    {
                        await context.ExerciseLog.AddAsync(exerciseLog);
                        exerciseLogsCreated++;
                    }
                    else if (exerciseLog.Equals(el))
                    {
                        context.Entry(el).CurrentValues.SetValues(exerciseLog);
                        exerciseLogsUpdated++;
                    }
                    else
                    {
                        exerciseLogsSkipped++;
                    }
                    exerciseLogsTotal++;
                    await context.ExerciseLog.AddAsync(exerciseLog);
                }
                await context.WorkoutLog.AddAsync(workoutLog);
            }
            var workoutEntity = await context.Workout.AddAsync(workout);

            // var test = workoutEntity.Entity;
        }

        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Exercise Sets. Created: {exerciseSetsCreated}, " +
            "Updated: {exerciseSetsUpdated}, Skipped: {exerciseSetsSkipped}, Failed: {exerciseSetsTotal - exerciseSetsUpdated - exerciseSetsSkipped}, Total: {exerciseSetsTotal}");
        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Exercise Logs. Created: {exerciseLogsCreated}, " +
            "Updated: {exerciseLogsUpdated}, Skipped: {exerciseLogsSkipped}, Failed: {exerciseLogsTotal - exerciseLogsUpdated - exerciseLogsSkipped}, Total: {exerciseLogsTotal}");

        // todo add logging for workout logs and workouts

        await context.SaveChangesAsync();
    }

    private static async Task<(int exerciseSetsCreated, int exerciseSetsUpdated, int exerciseSetsSkipped, int exerciseSetsTotal)>
    SeedExerciseSets(DataContext context, ExerciseLog exerciseLog)
    {
        var created = 0;
        var updated = 0;
        var skipped = 0;
        var total = 0;

        foreach (var exerciseSet in exerciseLog.ExerciseSets!)
        {
            var es = await context.ExerciseSet.SingleOrDefaultAsync(x => x.Id == exerciseSet.Id);
            if (es is null)
            {
                context.ExerciseSet.Add(exerciseSet);
                created++;
            }
            else if (exerciseSet.Equals(es))
            {
                context.Entry(es).CurrentValues.SetValues(exerciseSet);
                updated++;
            }
            else
            {
                skipped++;
            }
            total++;
        }
        return (created, updated, skipped, total);
    }
}