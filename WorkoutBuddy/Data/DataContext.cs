using Microsoft.EntityFrameworkCore;
using workouts;

namespace WorkoutBuddy.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<ExerciseDto> Exercises { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExerciseDto>()
                .HasMany(e => e.MuscleGroups);
        }

        
    }
}
