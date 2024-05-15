using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data.Seed;

public static class ProfileDataSeeder
{

    public static async Task SeedProfiles(this DataContext context)
    {
        Console.WriteLine("Seeding Profiles");

        var initialProfiles = new List<Profile>() {
            new(
                id : DatabaseHelper.CreatorId,
                userId : "24dqp5PO9iNN6Gh3zoNaY5NO8zp2",
                name : "The Creator",
                email : "test@test.com",
                profilePictureUrl : null
            )
    };

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var total = initialProfiles.Count;

        foreach (var profile in initialProfiles)
        {
            var p = context.Profile.SingleOrDefault(e => e.Id == profile.Id);

            if (p is null)
            {
                await context.Profile.AddAsync(profile);
                created++;
                Console.WriteLine($"Adding Profile: {profile.Name}");
            }
            else if (!p.Equals(profile))
            {
                context.Entry(p).CurrentValues.SetValues(profile);
                updated++;
                Console.WriteLine($"Update Profile: {profile.Name}");
            }
            else
            {
                skipped++;
                // Console.WriteLine($"Skipped Profile: {profile.Name}");
            }

        }
        await context.SaveChangesAsync();
        Console.WriteLine($"Result of {nameof(ProfileDataSeeder)}. Created: {created}, Updated: {updated}, Skipped: {skipped}, Total: {total}");
    }
}
