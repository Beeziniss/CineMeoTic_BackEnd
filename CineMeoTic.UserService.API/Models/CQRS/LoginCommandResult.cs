namespace CineMeoTic.UserService.API.Models.CQRS
{
    public sealed record LoginCommandResult
    (
        string AccessToken,
        string RefreshToken
    );
}