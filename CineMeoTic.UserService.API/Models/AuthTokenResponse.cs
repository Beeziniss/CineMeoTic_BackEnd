namespace CineMeoTic.UserService.API.Models;

public sealed record class AuthTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
