using WorkoutBuddy.Authentication;

namespace WorkoutBuddy.Features;

public class GoogleJwtProvider
{
    private readonly HttpClient _httpClient;

    public GoogleJwtProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthToken?> GetForCredentialsAsync(string email, string password)
    {
        var request = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync("", request);

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();

        return authToken;
    }
}