using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Models;

public sealed record class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public Gender Gender { get; set; } = Gender.Unspecified;
}
