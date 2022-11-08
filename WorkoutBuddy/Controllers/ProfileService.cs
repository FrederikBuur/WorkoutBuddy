using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers;

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly DataContext _dataContext;

    public ProfileService(ILogger<ProfileService> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    public ProfileDto? GetProfileByUserId(string userId) =>
        _dataContext.Profiles.SingleOrDefault(p => p.UserId == userId);
}
