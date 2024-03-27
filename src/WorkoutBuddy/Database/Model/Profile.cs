namespace WorkoutBuddy.Data.Model
{
    public class Profile : IEntityBase
    {
        /// <summary>
        /// This id comes from firebase / JWT token
        /// </summary>
        public string UserId { get; set; } = "";
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // EF Core needs empty constructor
        public Profile() { }

        public Profile(Guid? id, string userId, string? name, string? email, string? profilePictureUrl)
        {
            Id = id ?? Guid.NewGuid();
            UserId = userId;
            Name = name;
            Email = email;
            ProfilePictureUrl = profilePictureUrl;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is not Profile other) return false;

            return Id == other.Id
                && UserId == other.UserId
                && Name == other.Name
                && Email == other.Email
                && ProfilePictureUrl == other.ProfilePictureUrl;
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Id);
            hash.Add(UserId);
            hash.Add(Name);
            hash.Add(Email);
            hash.Add(ProfilePictureUrl);
            return hash.ToHashCode();
        }
    }
}
