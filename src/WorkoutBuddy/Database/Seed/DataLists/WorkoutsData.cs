
namespace WorkoutBuddy.Data.Seed;

public static class WorkoutsData
{
    public static readonly List<Workout> WorkoutsSeedData1 = new()
    {
        new(Guid.Parse("5b04ecaf-1305-48f0-82d6-2d2a53e1c2f9"),
            "My super cool full body workout",
            null,
            WorkoutLogsData.WorkoutLogSeedData1.Count
            ) {
                WorkoutDetailId = Guid.Parse("3a8fb17b-1b33-4b14-8a06-e74eb972480d"),
                ProfileId = DatabaseHelper.CreatorId,
                WorkoutLogs = WorkoutLogsData.WorkoutLogSeedData1.Select(wl=> {
                    wl.WorkoutId = Guid.Parse("5b04ecaf-1305-48f0-82d6-2d2a53e1c2f9");
                    return wl;
                }).ToList()
            }
    };
}