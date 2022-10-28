using Microsoft.EntityFrameworkCore;
using workouts;

namespace WorkoutBuddy.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            //Database.EnsureCreated();
        }

        public DbSet<ExerciseDto> Exercises { get; set; }
          
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Exercises");
            modelBuilder.Entity<ExerciseDto>()
                .ToContainer("Exercises")
                .HasPartitionKey(e => e.CreaterId)
                .HasMany(e => e.MuscleGroups);


        }
    }
}
