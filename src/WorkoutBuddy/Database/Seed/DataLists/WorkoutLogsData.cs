
using System.Globalization;

namespace WorkoutBuddy.Data.Seed;

public static class WorkoutLogsData
{
    public static readonly List<WorkoutLog> WorkoutLogSeedData1 = new(){
        new(
            Guid.Parse("3cf84732-dd0d-4640-be3c-fe3a89bb57bf"),
             DateTime.ParseExact("2011-03-21 13:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
        ){
            ExerciseLogs = ExerciseLogsData.ExerciseLogSeedData1.Select(el=> {
                el.WorkoutLogId = Guid.Parse("3cf84732-dd0d-4640-be3c-fe3a89bb57bf");
                return el;
            }).ToList()
        },
        new(
            Guid.Parse("1a2d0f69-4a4e-4280-9e58-952c577fd80a"),
             DateTime.ParseExact("2011-03-21 13:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
        ){
            ExerciseLogs = ExerciseLogsData.ExerciseLogSeedData1.Select(el=> {
                el.WorkoutLogId = Guid.Parse("1a2d0f69-4a4e-4280-9e58-952c577fd80a");
                return el;
            }).ToList()
        }
    };
}