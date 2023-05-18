using WorkoutBuddy.Data.Model;

namespace WorkoutBuddy.Controllers;

public interface IProfileService
{
    Profile? GetProfile();
}