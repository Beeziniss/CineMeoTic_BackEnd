using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Models.CQRS;

public sealed record UserProfileQuery
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public IEnumerable<UserRole> Roles { get; init; } = null!;
}
