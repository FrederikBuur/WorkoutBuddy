using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Data.Seed;
using workouts;

namespace WorkoutBuddy.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<ProfileDto> Profiles { get; set; }
    public DbSet<ExerciseDto> Exercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Profile
        modelBuilder.Entity<ProfileDto>()
            .ToContainer("Profiles")
            .HasPartitionKey(p => p.UserId);

        // Exercise
        modelBuilder.HasDefaultContainer("Exercises");
        modelBuilder.Entity<ExerciseDto>()
            .ToContainer("Exercises")
            .HasPartitionKey(e => e.CreatorId)
            .Property(mg => mg.PrimaryMuscleGroup)
            .HasConversion(new EnumToStringConverter<MuscleGroupType>());
        modelBuilder.Entity<ExerciseDto>()
            .Property(mg => mg.SecondaryMuscleGroups)
            .HasConversion(
                mg => string.Join(",", mg),
                mg => mg.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Enum.Parse<MuscleGroupType>(x))
                    .ToArray(), 
                new ValueComparer<ICollection<MuscleGroupType>>(
                    (mg1, mg2) => mg1.SequenceEqual(mg2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
            );

        // Workout

        // Seeding
        modelBuilder.Entity<ProfileDto>()
            .SeedProfiles();
        modelBuilder.Entity<ExerciseDto>()
            .SeedExercises();
    }
}
