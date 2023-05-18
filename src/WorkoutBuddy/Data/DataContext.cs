using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkoutBuddy.Controllers.Exercise.Model;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.EFConverters;

namespace WorkoutBuddy.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        // seems like migrations are not supported for cosmos. makes sence since it is document based
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    public DbSet<Model.Profile> Profiles => base.Set<Model.Profile>();
    public DbSet<Model.Exercise> Exercises => base.Set<Model.Exercise>();
    public DbSet<Workout> Workouts => Set<Workout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Profile
        modelBuilder.Entity<Profile>()
            .ToContainer("Profiles")
            .HasPartitionKey(p => p.UserId);

        // Exercise
        modelBuilder.Entity<Exercise>()
            .ToContainer("Exercises")
            .HasPartitionKey(e => e.CreatorId)
            .Property(mg => mg.PrimaryMuscleGroup)
            .HasConversion(new EnumToStringConverter<MuscleGroupType>());
        modelBuilder.Entity<Model.Exercise>()
            .Property(mg => mg.SecondaryMuscleGroups)
            .HasConversion(
                mg => string.Join(",", mg),
                mg => mg.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Enum.Parse<MuscleGroupType>(x))
                    .ToArray(),
                new ValueComparer<ICollection<MuscleGroupType>>(
                    (mg1, mg2) => mg1!.SequenceEqual(mg2!),
                    c => c.Aggregate(0, (int a, MuscleGroupType v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList())
            );

        // Workout
        modelBuilder.Entity<Workout>(entity =>
        {
            entity.ToContainer("Workouts")
                .HasPartitionKey(w => w.Owner);

            entity.HasMany(e => e.Exercises);
        });


        // Seeding - this method is onlr working with migrations
        // modelBuilder.Entity<ProfileDto>()
        //     .SeedProfiles();
        // modelBuilder.Entity<ExerciseDto>()
        //     .SeedExercises();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeUtcConverter>();
    }
}
