namespace CineMeoTic.UserService.API.Models.CQRS;

public sealed record LoginCommandResult
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}