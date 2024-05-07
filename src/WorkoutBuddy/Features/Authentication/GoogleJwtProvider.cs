using WorkoutBuddy.Authentication;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features;

public class GoogleJwtProvider
{
    private readonly HttpClient _httpClient;

    public GoogleJwtProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<AuthToken?>> GetForCredentialsAsync(string email, string password)
    {
        var request = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync("", request);

        if (!response.IsSuccessStatusCode)
        {
            return new Result<AuthToken?>(Error.BadRequest("Provided credentials are invalid"));
        }

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();

        return authToken;
    }
}