using BuildingBlocks.Models;
using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Data;

public sealed class User : Auditable
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
    public Gender Gender { get; set; } = Gender.Unspecified;
    public string? PhoneNumber { get; set; }
    public string Avatar { get; set; } = null!;
}
