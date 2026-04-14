using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Models;

public sealed record class UserInfoInternalResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public IEnumerable<UserRole> Roles { get; init; } = null!;
}
