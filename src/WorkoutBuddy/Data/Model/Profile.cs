namespace WorkoutBuddy.Data.Model
{
    public class Profile : IEntityBase
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = "";
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public override bool Equals(object? obj)
        {
            var other = obj as Profile;
            if (other is null) return false;

            return this.Id == other.Id &&
            this.UserId == other.UserId &&
            this.Name == other.Name &&
            this.Email == other.Email &&
            this.ProfilePictureUrl == other.ProfilePictureUrl;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
