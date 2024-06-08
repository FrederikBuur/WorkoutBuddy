using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util.ErrorHandling;
using WorkoutBuddy.Services;

namespace WorkoutBuddy.Features;

public class ProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly DataContext _dataContext;
    private readonly UserService _userService;

    public ProfileService(
        ILogger<ProfileService> logger,
        DataContext dataContext,
        UserService userService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _userService = userService;
    }

    public Result<Profile> GetProfile()
    {
        var userId = _userService.Id;
        var profile = _dataContext.Profile.SingleOrDefault(p => p.UserId == userId);
        if (profile is null)
        {
            _logger.LogError($"Missing user. user from http context: {userId ?? "null"}, could not be found in Profile db");
            return new Result<Profile>(Error.Unauthorized("Missing user"));
        }
        else
            return new Result<Profile>(profile);
    }

    public async Task<Result<Profile>> CreateProfile(ProfileRequest profileReq)
    {
        var profileResult = GetProfile();
        if (profileResult.IsFaulted)
            return new Result<Profile>(profileResult.Error);

        var existingprofile = _dataContext.Profile.SingleOrDefault(p => p.Email == profileReq.Email);

        if (existingprofile is not null) return new Result<Profile>(Error.BadRequest("Profile with email already exists"));

        var profile = new Profile(
            id: Guid.NewGuid(),
            userId: profileResult.Value.UserId,
            name: profileReq.Name,
            email: profileReq.Email,
            profilePictureUrl: profileReq.ProfilePictureUrl
        );

        var createdProfile = await _dataContext.Profile.AddAsync(profile);

        if (createdProfile?.Entity is not null)
        {
            return new Result<Profile>(createdProfile.Entity);
        }
        else
        {
            return new Result<Profile>(Error.InternalServerError());
        }
    }
}
