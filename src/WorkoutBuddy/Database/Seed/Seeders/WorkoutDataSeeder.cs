using Microsoft.EntityFrameworkCore;
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Data.Seed;

public static class WorkoutAndSessionDataSeeder
{
    public static async Task SeedWorkoutAndSessionDataSeeder(this DataContext context)//, IServiceScope scope)
    {
        var workouts = WorkoutsData.WorkoutsSeedData1;

        var workoutsCreated = 0;
        var workoutsUpdated = 0;
        var workoutsSkipped = 0;
        var workoutsTotal = 0;

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

                    // upsert exerciseset
                    var exerciseSetsStatus = await SeedExerciseSets(context, exerciseLog);
                    exerciseSetsCreated += exerciseSetsStatus.exerciseSetsCreated;
                    exerciseSetsUpdated += exerciseSetsStatus.exerciseSetsUpdated;
                    exerciseSetsSkipped += exerciseSetsStatus.exerciseSetsSkipped;
                    exerciseSetsTotal += exerciseSetsStatus.exerciseSetsTotal;

                    // upsert exerciselog
                    var el = await context.ExerciseLog.SingleOrDefaultAsync(x => x.Id == exerciseLog.Id);
                    if (el is null)
                    {
                        context.ExerciseLog.Add(exerciseLog);
                        exerciseLogsCreated++;
                    }
                    else if (!exerciseLog.Equals(el))
                    {
                        context.Entry(el).CurrentValues.SetValues(exerciseLog);
                        exerciseLogsUpdated++;
                    }
                    else
                    {
                        exerciseLogsSkipped++;
                    }
                    exerciseLogsTotal++;
                    // await context.ExerciseLog.AddAsync(exerciseLog);
                }

                // upsert workoutlog
                var wl = await context.WorkoutLog.SingleOrDefaultAsync(x => x.Id == workoutLog.Id);
                if (wl is null)
                {
                    await context.WorkoutLog.AddAsync(workoutLog);
                    workoutLogsCreated++;
                }
                else if (!workoutLog.Equals(wl))
                {
                    context.Entry(wl).CurrentValues.SetValues(workoutLog);
                    workoutLogsUpdated++;
                }
                else
                {
                    workoutLogsSkipped++;
                }
                workoutLogsTotal++;

            }
            // upsert workout
            var w = await context.Workout.SingleOrDefaultAsync(x => x.Id == workout.Id);
            if (w is null)
            {
                context.Workout.Add(workout);
                workoutsCreated++;
            }
            else if (!workout.Equals(w))
            {
                context.Entry(w).CurrentValues.SetValues(workout);
                workoutsUpdated++;
            }
            else
            {
                workoutsSkipped++;
            }
            workoutsTotal++;
        }

        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Exercise Sets. \n\tCreated: {exerciseSetsCreated}, " +
            $"\tUpdated: {exerciseSetsUpdated}, \tSkipped: {exerciseSetsSkipped}, \tFailed: {exerciseSetsTotal - exerciseSetsUpdated - exerciseSetsSkipped}, \tTotal: {exerciseSetsTotal}");
        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Exercise Logs. \n\tCreated: {exerciseLogsCreated}, " +
            $"\tUpdated: {exerciseLogsUpdated}, \tSkipped: {exerciseLogsSkipped}, \tFailed: {exerciseLogsTotal - exerciseLogsUpdated - exerciseLogsSkipped}, \tTotal: {exerciseLogsTotal}");
        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Workout Logs. \n\tCreated: {workoutLogsCreated}, " +
            $"\tUpdated: {workoutLogsUpdated}, \tSkipped: {workoutLogsSkipped}, \tFailed: {workoutLogsTotal - workoutLogsUpdated - workoutLogsSkipped}, \tTotal: {workoutLogsTotal}");
        Console.WriteLine($"Result of {nameof(SeedWorkoutAndSessionDataSeeder)} Workouts. \n\tCreated: {workoutsCreated}, " +
            $"\tUpdated: {workoutsUpdated}, \tSkipped: {workoutsSkipped}, \tFailed: {workoutsTotal - workoutsUpdated - workoutsSkipped}, \tTotal: {workoutsTotal}");

        await context.SaveChangesAsync();
    }

    private static async Task<(
        int exerciseSetsCreated,
        int exerciseSetsUpdated,
        int exerciseSetsSkipped,
        int exerciseSetsTotal)>
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
            else if (!exerciseSet.Equals(es))
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