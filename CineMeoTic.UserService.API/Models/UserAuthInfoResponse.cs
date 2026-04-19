namespace CineMeoTic.UserService.API.Models;

public sealed record UserAuthInfoResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
}
