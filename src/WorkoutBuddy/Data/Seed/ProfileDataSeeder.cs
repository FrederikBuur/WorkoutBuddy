using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data.Seed;

public static class ProfileDataSeeder
{

    public static async Task SeedProfiles(this DataContext context)
    {
        Console.WriteLine("Seeding Profiles");

        var initialProfiles = new List<Profile>() {
            new Profile()
            {
                Id = DatabaseHelper.CreatorId,
                UserId = "24dqp5PO9iNN6Gh3zoNaY5NO8zp2",
                Name = "The Creator",
                Email = "test@test.com",
                ProfilePictureUrl = null
            }
        };

        foreach (var profile in initialProfiles)
        {
            var p = context.Profiles.SingleOrDefault(e => e.Id == profile.Id);

            if (p is null)
            {
                await context.Profiles.AddAsync(profile);
                Console.WriteLine($"Adding Profile: {profile.Name}");
            }
            else if (!p.Equals(profile))
            {
                context.Entry(p).CurrentValues.SetValues(profile);
                Console.WriteLine($"Update Profile: {profile.Name}");
            }
            else
            {
                Console.WriteLine($"Skipped Profile: {profile.Name}");
            }

        }
        await context.SaveChangesAsync();
    }
}
