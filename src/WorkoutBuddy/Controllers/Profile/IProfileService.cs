using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers;

public interface IProfileService
{
    ProfileDto? GetProfile();
}