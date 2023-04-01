namespace WorkoutBuddy.Data.Model
{
    public class ProfileDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
