namespace CineMeoTic.UserService.API.Models;

public sealed record UserInfoResponse
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Avatar { get; init; } = null!;
}
