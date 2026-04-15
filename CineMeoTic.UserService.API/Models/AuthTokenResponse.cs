namespace CineMeoTic.UserService.API.Models;

public sealed record AuthTokenResponse
(
    string AccessToken,
    string RefreshToken
);
