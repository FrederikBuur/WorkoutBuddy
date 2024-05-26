using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util.ErrorHandling;
using WorkoutBuddy.Services;

namespace WorkoutBuddy.Features;

public class ProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly DataContext _dataContext;
    private readonly UserService _userService;

    public ProfileService(ILogger<ProfileService> logger, DataContext dataContext, UserService userService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _userService = userService;
    }

    public Result<Profile> GetProfileResult()
    {
        var userId = _userService.Id;
        var profile = _dataContext.Profile.SingleOrDefault(p => p.UserId == userId);
        if (profile is null)
        {
            _logger.LogError($"Missing user. user from http context: {userId}, could not be found in Profile db");
            return new Result<Profile>(Error.Unauthorized("Missing user"));
        }
        else
            return new Result<Profile>(profile);
    }
}
