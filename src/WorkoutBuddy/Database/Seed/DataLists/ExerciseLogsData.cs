namespace WorkoutBuddy.Data.Seed;

public static class ExerciseLogsData
{
    public static readonly List<ExerciseLog> ExerciseLogSeedData1 = new(){
        new(id: Guid.Parse("bd92406a-0a38-481c-addb-50d5f0de1f6a")){
            ExerciseSets = ExerciseSetsData.ExerciseSetsSeedData1.Select(es=>{
                es.ExerciseLogId = Guid.Parse("bd92406a-0a38-481c-addb-50d5f0de1f6a");
                return es;
            }).ToList()
        },
        new(id: Guid.Parse("2a96f421-6e34-4d31-8e46-41e3c49e1dae")){
            ExerciseSets = ExerciseSetsData.ExerciseSetsSeedData2.Select(es=>{
                es.ExerciseLogId = Guid.Parse("2a96f421-6e34-4d31-8e46-41e3c49e1dae");
                return es;
            }).ToList()
        },
        new(id: Guid.Parse("81213093-8cd2-4259-9c06-baa15f81c133")){
            ExerciseSets = ExerciseSetsData.ExerciseSetsSeedData3.Select(es=>{
                es.ExerciseLogId = Guid.Parse("81213093-8cd2-4259-9c06-baa15f81c133");
                return es;
            }).ToList()
        }
    };

    public static readonly List<ExerciseLog> ExerciseLogSeedData2 = new(){
        new(id: Guid.Parse("18746a88-1382-43b8-83da-2832ba4954b7")){
            ExerciseSets = ExerciseSetsData.ExerciseSetsSeedData4.Select(es=>{
                es.ExerciseLogId = Guid.Parse("18746a88-1382-43b8-83da-2832ba4954b7");
                return es;
            }).ToList()
        },
        new(id: Guid.Parse("e9f9532b-80b9-4d01-a531-324ec580bf53")){
            ExerciseSets = ExerciseSetsData.ExerciseSetsSeedData5.Select(es=>{
                es.ExerciseLogId = Guid.Parse("e9f9532b-80b9-4d01-a531-324ec580bf53");
                return es;
            }).ToList()
        }
    };
}