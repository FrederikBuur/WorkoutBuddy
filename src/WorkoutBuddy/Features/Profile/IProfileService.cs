using WorkoutBuddy.Features.ErrorHandling;

namespace WorkoutBuddy.Features;

public interface IProfileService
{
    Data.Model.Profile? GetProfile();
    HttpResponseException? ProfileMissingAsException(out Data.Model.Profile? profile);
}