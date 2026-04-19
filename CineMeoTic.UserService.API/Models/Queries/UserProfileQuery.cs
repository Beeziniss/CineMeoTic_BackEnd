namespace CineMeoTic.UserService.API.Models.Queries;

public sealed record UserProfileQuery
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public IEnumerable<string> Roles { get; init; } = null!;
}
