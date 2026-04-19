namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record LoginCommandResult
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}