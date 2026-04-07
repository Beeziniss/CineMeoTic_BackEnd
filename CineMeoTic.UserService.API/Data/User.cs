using CineMeoTic.Common.Models;
using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Data;

public sealed class User : Auditable
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string? Name { get; set; }
    public Gender Gender { get; set; } = Gender.Unspecified;
    public string? PhoneNumber { get; set; }

    public ICollection<Role> Roles { get; set; } = null!;
}
