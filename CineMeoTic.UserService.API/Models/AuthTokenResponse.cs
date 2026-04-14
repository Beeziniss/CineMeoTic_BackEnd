using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models;

public sealed record class AuthTokenResponse : IQuery<AuthTokenResponse>
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
