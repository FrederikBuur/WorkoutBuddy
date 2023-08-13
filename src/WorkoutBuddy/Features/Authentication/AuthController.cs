using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WorkoutBuddy.Authentication;

/**
 * setup firebase auth with swagger ui
 * https://stackoverflow.com/questions/61540706/configure-swagger-authentication-with-firebase-google-in-net-core
**/

[AllowAnonymous]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("v1/auth")]
public class AuthController : Controller
{
    private readonly FirebaseAuthOptions _settings;

    public AuthController(IOptions<FirebaseAuthOptions> settings)
    {
        _settings = settings.Value;
    }

    [HttpPost]
    public async Task<ActionResult> GetTokenFromGoogle([FromForm] LoginInfo loginInfo)
    {
        var apiKey = _settings.FirebaseApiKey ?? throw new ArgumentException("Missing firebase api key");
        string uri = $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={apiKey}";
        using (HttpClient client = new HttpClient())
        {
            var fireBaseLoginInfo = new FireBaseLoginInfo
            {
                Email = loginInfo.Username,
                Password = loginInfo.Password
            };
            var result = await client.PostAsJsonAsync<FireBaseLoginInfo, GoogleToken>(uri, fireBaseLoginInfo);
            var token = new Token
            {
                token_type = "Bearer",
                access_token = result?.idToken,
                id_token = result?.idToken,
                expires_in = int.Parse(result?.expiresIn ?? "0"),
                refresh_token = result?.refreshToken

            };
            return Ok(token);
        }
    }
}

public class LoginInfo
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

}

public class FireBaseLoginInfo
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool ReturnSecureToken { get; set; } = true;

}

public class GoogleToken
{
    public string? kind { get; set; }
    public string? localId { get; set; }
    public string? email { get; set; }
    public string? displayName { get; set; }
    public string? idToken { get; set; }
    public bool registered { get; set; }
    public string? refreshToken { get; set; }
    public string? expiresIn { get; set; }
}


public class Token
{
    internal string? refresh_token;
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public int ext_expires_in { get; set; }
    public string? access_token { get; set; }
    public string? id_token { get; set; }
}
