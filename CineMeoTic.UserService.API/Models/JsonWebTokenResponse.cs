namespace CineMeoTic.UserService.API.Models;

public sealed record JsonWebTokenResponse
(
    string AccessToken,
    string RefreshToken
);
