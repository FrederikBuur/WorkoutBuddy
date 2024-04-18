using System.ComponentModel.DataAnnotations;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Authentication;

/**
 * setup firebase auth with swagger ui
 * https://stackoverflow.com/questions/61540706/configure-swagger-authentication-with-firebase-google-in-net-core
**/

[AllowAnonymous]
[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly GoogleJwtProvider _googleJwtProvider;

    public AuthController(GoogleJwtProvider googleJwtProvider)
    {
        _googleJwtProvider = googleJwtProvider;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody][Required] LoginInfo loginInfo)
    {

        var userArgs = new UserRecordArgs
        {
            Email = loginInfo.Username,
            Password = loginInfo.Password
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

        return Ok(userRecord.Uid);
    }

    [HttpPost("login/email-password")]
    public async Task<ActionResult> LoginWithEmailPassword([FromBody][Required] LoginInfo loginInfo)
    {
        var token = await _googleJwtProvider.GetForCredentialsAsync(loginInfo.Username, loginInfo.Password);

        if (token is null)
            return BadRequest("Getting token failed");

        return Ok(token);
    }
}
