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
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ExerciseDetail> ExerciseDetails => Set<ExerciseDetail>();
    public DbSet<WorkoutDetail> WorkoutDetails => Set<WorkoutDetail>();
    public DbSet<Workout> Workouts => Set<Workout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Profile
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profiles")
            .HasKey(p => p.Id);

            entity.HasIndex(p => p.UserId);
        });

        // Exercise Detail
        modelBuilder.Entity<ExerciseDetail>(entity =>
        {
            entity.ToTable("ExerciseDetails")
            .HasKey(ed => ed.Id);
        });

        // Workout Detail
        modelBuilder.Entity<WorkoutDetail>(entity =>
        {
            entity.ToTable("WorkoutDetails")
            .HasKey(wd => wd.Id);

            entity.HasMany(w => w.Exercises)
            .WithMany()
            .UsingEntity<ExerciseDetailWorkoutDetail>();

        });

        // Workout
        modelBuilder.Entity<Workout>(entity =>
        {
            entity.ToTable("Workouts")
            .HasKey(w => w.Id);

            entity.HasOne(w => w.Profile)
            .WithOne()
            .HasForeignKey<Workout>(w => w.ProfileId);

            entity.HasOne(w => w.WorkoutDetail)
            .WithOne()
            .HasForeignKey<Workout>(w => w.WorkoutDetailId);
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeUtcConverter>();
    }

    public override int SaveChanges()
    {
        UpdateEntityBaseFields(); // setup for base entity properties
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntityBaseFields(); // setup for base entity properties
        return base.SaveChangesAsync(true, cancellationToken);
    }

    private void UpdateEntityBaseFields()
    {
        var now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity is null) continue;
            if (changedEntity.Entity is not IEntityBase entity)
                throw new Exception($"Entity must implement: {nameof(IEntityBase)}");

            switch (changedEntity.State)
            {
                case EntityState.Added:
                    if (entity.Id == default) entity.Id = Guid.NewGuid();
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
