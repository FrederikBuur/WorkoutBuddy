
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using WorkoutBuddy.Util.ErrorHandling;

namespace WorkoutBuddy.Features.Authentication;

public class AuthService
{
    private readonly FirebaseApp _firebaseApp;
    private readonly GoogleJwtProvider _googleJwtProvider;
    private readonly GoogleRefreshProvider _googleRefreshProvider;
    private readonly ProfileService _profileService;

    public AuthService(ProfileService profileService,
    GoogleJwtProvider googleJwtProvider,
    GoogleRefreshProvider googleRefreshProvider)
    {
        _firebaseApp = FirebaseApp.DefaultInstance;
        _profileService = profileService;
        _googleJwtProvider = googleJwtProvider;
        _googleRefreshProvider = googleRefreshProvider;
    }

    // register with email/password - can be done from app
    // public async Task<Result<IdentityAuthToken>> 

    // register with google login - can be done from app

    public async Task<Result<IdentityAuthToken>> LoginWithEmail(LoginRequest loginRequest)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(loginRequest.Email);

        if (!user.EmailVerified)
            return new Result<IdentityAuthToken>(Error.Unauthorized("User is not email verified"));

        var verificationLionk = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(loginRequest.Email);

        var tokenResult = await _googleJwtProvider.GetForCredentialsAsync(
            loginRequest.Email,
            loginRequest.Password);

        if (tokenResult.IsFaulted)
            return new Result<IdentityAuthToken>(tokenResult.Error);

        return tokenResult;
    }

    public async Task<Result<RefreshJwtResponse>> RefreshJwtToken(RefreshJwtRequest refreshTokenRequest)
    {
        var newToken = await _googleRefreshProvider.RefreshJwtToken(refreshTokenRequest);

        if (newToken.IsFaulted)
            return new Result<RefreshJwtResponse>(newToken.Error);

        return newToken;
    }
}