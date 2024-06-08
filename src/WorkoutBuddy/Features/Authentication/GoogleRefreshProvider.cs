using WorkoutBuddy.Features.Authentication;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features;

public class GoogleRefreshProvider
{
    private readonly ILogger<GoogleJwtProvider> _logger;
    private readonly HttpClient _httpClient;

    public GoogleRefreshProvider(ILogger<GoogleJwtProvider> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result<RefreshJwtResponse>> RefreshJwtToken(RefreshJwtRequest refreshTokenRequest)
    {
        var requestBody = new
        {
            grant_type = "refresh_token",
            refresh_token = refreshTokenRequest.RefreshToken
        };

        var response = await _httpClient.PostAsJsonAsync("", requestBody);
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Error refreshing GoogleJwt {errorMsg}");
            return new Result<RefreshJwtResponse>(Error.BadRequest("Provided refresh token is invalid"));
        }

        var test = await response.Content.ReadAsStringAsync();

        var authToken = await response.Content.ReadFromJsonAsync<RefreshJwtResponse>();

        if (authToken is null) return new Result<RefreshJwtResponse>(Error.InternalServerError("Error parsing auth token from google auth"));

        return authToken;
    }
}
