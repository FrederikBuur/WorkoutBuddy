using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutBuddy.Data.Seed;
using workouts;

namespace WorkoutBuddy.Data;

public static class ExerciseDataSeeder
{
    public static DataBuilder<ExerciseDto> SeedExercises(this EntityTypeBuilder<ExerciseDto> builder)
    {
        // exercise db:
        // https://www.jefit.com/exercises
        // https://exrx.net/Lists/Directory
        // https://www.bodybuilding.com/exercises/

        var creatorId = ProfileDataSeeder.CreatorId;

        var deadlift = new ExerciseDto()
        {
            Id = Guid.Parse("9f1a7d66-e070-424b-aa3b-dc1ff3cd3bff"),
            CreatorId = creatorId,
            Name = "Deadlift",
            Description = "The barbell deadlift is a classic bodybuilding exercise meant for putting on mass and building overall strength throughout the entire body.",
            PrimaryMuscleGroup = MuscleGroupType.UpperBack,
            SecondaryMuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Hamstring, MuscleGroupType.Glutes }
        };

        var benchpress = new ExerciseDto()
        {
            Id = Guid.Parse("8126c195-4f25-47a6-be90-9333ee061de6"),
            CreatorId = creatorId,
            Name = "Bench press",
            Description = "The bench press is a compound exercise that builds strength and muscle in the chest and triceps. When many people think of listing, the bench press is often the first exercise that comes to mind",
            PrimaryMuscleGroup = MuscleGroupType.Chest,
            SecondaryMuscleGroups = new List<MuscleGroupType> { MuscleGroupType.Tricep, MuscleGroupType.Shoulder }
        };

        return builder.HasData(
            deadlift,
            benchpress
        );
    }
}
