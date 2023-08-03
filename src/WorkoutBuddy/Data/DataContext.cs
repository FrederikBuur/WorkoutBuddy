using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
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
            .Property(mg => mg.MuscleGroups)
            // .HasConversion(
            //     mg => string.Join(",", mg),
            //     mg => mg.Split(",", StringSplitOptions.RemoveEmptyEntries)
            //         .Select(x => Enum.Parse<MuscleGroupType>(x))
            //         .ToArray(),
            //     new ValueComparer<IEnumerable<MuscleGroupType>>(
            //         (mg1, mg2) => mg1!.SequenceEqual(mg2!),
            //         c => c.Aggregate(0, (int a, MuscleGroupType v) => HashCode.Combine(a, v.GetHashCode())),
            //         c => c.ToList())
            // )
            ;

        // Workout
        modelBuilder.Entity<Workout>(entity =>
        {
            entity.ToContainer("Workouts")
                .HasPartitionKey(w => w.Owner);
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

    public override int SaveChanges()
    {
        UpdateEntityBaseFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntityBaseFields();
        return base.SaveChangesAsync(true, cancellationToken);
    }

    private void UpdateEntityBaseFields()
    {
        var now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is not IEntityBase entity) throw new Exception($"Entity must implement: {nameof(IEntityBase)}");

            switch (changedEntity.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = now;
                    entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
