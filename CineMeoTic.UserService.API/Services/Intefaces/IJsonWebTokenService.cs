using System.Security.Claims;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IJsonWebTokenService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) access token containing the specified claims and an optional expiration
    /// period.
    /// </summary>
    /// <remarks>The generated token can be used to authenticate API requests or authorize access to
    /// protected resources. Ensure that the claims provided accurately represent the user's identity and
    /// permissions.</remarks>
    /// <param name="claims">The collection of claims to include in the generated access token. Cannot be null or empty.</param>
    /// <param name="expirationInDays">The number of days until the access token expires. Must be a positive integer. The default is 7 days.</param>
    /// <returns>A string representing the generated JWT access token.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims, int expirationInDays = 7);

    /// <summary>
    /// Generates a new refresh token for use in authentication workflows.
    /// </summary>
    /// <remarks>Refresh tokens are typically long-lived credentials used to obtain new access tokens
    /// without requiring the user to re-authenticate. Store refresh tokens securely and transmit them only over
    /// secure channels.</remarks>
    /// <returns>A string containing the newly generated refresh token. The token is suitable for securely identifying a user
    /// session when requesting new access tokens.</returns>
    string GenerateRefreshToken();

}
