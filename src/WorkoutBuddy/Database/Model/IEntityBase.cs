
namespace WorkoutBuddy.Data.Model;

public class IEntityBase
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}