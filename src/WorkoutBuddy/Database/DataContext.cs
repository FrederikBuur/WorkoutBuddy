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
            entity.HasKey(p => p.Id);

            entity.HasIndex(p => p.UserId);
        });

        // Exercise Detail
        modelBuilder.Entity<ExerciseDetail>(entity =>
        {
            entity.HasKey(ed => ed.Id);
        });

        // Workout Detail
        modelBuilder.Entity<WorkoutDetail>(entity =>
        {
            entity.HasKey(wd => wd.Id);

            entity.HasMany(w => w.Exercises)
            .WithMany()
            .UsingEntity<ExerciseDetailWorkoutDetail>();

        });

        // Workout
        modelBuilder.Entity<Workout>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.HasOne(w => w.Profile)
                .WithMany()
                .HasForeignKey(w => w.ProfileId);

            entity.HasOne(w => w.WorkoutDetail)
                .WithMany()
                .HasForeignKey(w => w.WorkoutDetailId);

            entity.HasMany(w => w.WorkoutLogs)
                .WithOne();
        });

        // WorkoutLog
        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.HasKey(wl => wl.Id);

            entity.HasOne(wl => wl.Workout)
                .WithMany()
                .HasForeignKey(wl => wl.WorkoutId);

            entity.HasMany(wl => wl.ExerciseLog)
            .WithOne();
        });

        // ExercuseLog
        modelBuilder.Entity<ExerciseLog>(entity =>
        {
            entity.HasKey(el => el.Id);

            entity.HasOne(el => el.WorkoutLog)
            .WithMany()
            .HasForeignKey(el => el.WorkoutLogId);

            entity.HasOne(el => el.ExerciseDetail)
            .WithMany()
            .HasForeignKey(el => el.ExerciseDetailId);

            entity.HasMany(el => el.ExerciseSets)
            .WithOne(es => es.ExerciseLog);
        });

        // ExerciseSet
        modelBuilder.Entity<ExerciseSet>(entity =>
        {
            entity.HasKey(es => es.Id);

            entity.HasOne(es => es.ExerciseLog)
            .WithMany()
            .HasForeignKey(es => es.ExerciseLogId);
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
