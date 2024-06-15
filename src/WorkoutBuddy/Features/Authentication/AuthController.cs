using System.ComponentModel.DataAnnotations;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Util;

namespace WorkoutBuddy.Features.Authentication;

/**
 * setup firebase auth with swagger ui
 * https://stackoverflow.com/questions/61540706/configure-swagger-authentication-with-firebase-google-in-net-core
**/

[AllowAnonymous]
[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register/email-password")]
    public async Task<ActionResult<Profile>> RegisterUserEmailPassword([FromBody] LoginRequest loginInfo)
    {
        var profileResult = await _authService.RegisterEmailPassword(loginInfo);

        return profileResult.ToActionResult((p) => p);
    }

    [HttpPost("login/email-password")]
    public async Task<ActionResult<IdentityAuthToken>> LoginWithEmailPassword(
        [FromBody] LoginRequest loginRequest)
    {
        var tokenResult = await _authService.LoginWithEmail(loginRequest);

        return tokenResult.ToActionResult((t) => t);
    }

    [HttpPost("register/google")]
    public async Task<ActionResult<UserRecord>> RegisterUserGoogle()
    {
        throw new NotImplementedException();
    }

    [HttpPost("refresh-jwt")]
    public async Task<ActionResult<RefreshJwtResponse>> RefreshJwt(
        [FromBody] RefreshJwtRequest refreshJwtReq
    )
    {
        var newTokenResult = await _authService.RefreshJwtToken(refreshJwtReq);

        return newTokenResult.ToActionResult((t) => t);
    }
}
