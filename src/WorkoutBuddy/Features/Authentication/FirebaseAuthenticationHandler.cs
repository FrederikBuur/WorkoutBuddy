using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace workouts;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private FirebaseApp _firebaseApp;

    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseApp firebaseApp
    ) : base(options, logger, encoder, clock)
    {
        _firebaseApp = firebaseApp;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Bearer token");
        }

        string? bearerToken = Context.Request.Headers["Authorization"];
        if (bearerToken is null || !bearerToken.StartsWith("Bearer "))
        {
            return AuthenticateResult.Fail("Invalid scheme");
        }

        var token = bearerToken.Substring("Bearer ".Length);
        try
        {
            var firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);
            var authTicket = new AuthenticationTicket(
            new ClaimsPrincipal(
                new List<ClaimsIdentity>()
                {
                        new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(FirebaseAuthenticationHandler))
                }
            ),
            JwtBearerDefaults.AuthenticationScheme
        );
            return AuthenticateResult.Success(authTicket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }

    private IEnumerable<Claim>? ToClaims(IReadOnlyDictionary<string, object> claims)
    {
        var claimsList = new List<Claim>
            {
                new Claim("id", claims["user_id"].ToString()!),
                new Claim("email", claims["email"].ToString()!)
            };

        if (claims.TryGetValue("name", out var nameValue))
        {
            claimsList.Add(new Claim("name", nameValue.ToString()!));
        }
        return claimsList;
    }
}
