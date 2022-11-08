using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Data.Seed;

public static class ProfileDataSeeder
{
    public static readonly Guid CreatorId = Guid.Parse("ea21cae2-728c-4679-a14f-988e1ccfcd64");

    public static DataBuilder<ProfileDto> SeedProfiles(this EntityTypeBuilder<ProfileDto> builder)
    {
        var creatorProfile = new ProfileDto()
        {
            Id = Guid.Parse("bf64a313-03bf-4758-a1dd-60460e0b2807"),
            UserId = "24dqp5PO9iNN6Gh3zoNaY5NO8zp2",
            Name = "The Creator",
            Email = "test@test.com",
            ProfilePictureUrl = null
        };

        return builder.HasData(creatorProfile);
    }
}
