using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Controllers;

public interface IProfileService
{
    Profile? GetProfile();
    HttpResponseException? ProfileMissingAsException(out Profile? profile);
}