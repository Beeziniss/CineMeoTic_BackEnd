using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Models;

public sealed record class UserModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string? Name { get; init; }
    public UserGender Gender { get; set; } = UserGender.Unspecified;
    public string? PhoneNumber { get; init; }
}
