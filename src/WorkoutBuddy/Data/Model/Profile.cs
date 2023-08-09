namespace WorkoutBuddy.Data.Model
{
    public class Profile : IEntityBase
    {
        public string UserId { get; set; } = "";
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not Profile other) return false;

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
