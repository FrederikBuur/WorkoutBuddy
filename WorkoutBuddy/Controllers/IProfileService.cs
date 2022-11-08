using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers
{
    public interface IProfileService
    {
        ProfileDto? GetProfileByUserId(string userId);
    }
}