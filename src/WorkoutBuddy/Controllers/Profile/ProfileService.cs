using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Services;

namespace WorkoutBuddy.Controllers;

public class ProfileService : IProfileService
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

    public Profile? GetProfile()
    {
        var userId = _userService.Id;
        return _dataContext.Profiles.SingleOrDefault(p => p.UserId == userId);
    }
}
