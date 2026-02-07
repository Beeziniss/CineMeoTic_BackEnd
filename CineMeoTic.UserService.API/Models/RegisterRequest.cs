using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Models;

public sealed record class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Name { get; set; }
    public UserGender Gender { get; set; } = UserGender.Unspecified;
    public string? PhoneNumber { get; set; }
}
