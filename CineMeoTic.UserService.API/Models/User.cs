using CineMeoTic.Common.Models;
using CineMeoTic.UserService.API.Models.Enums;

namespace CineMeoTic.UserService.API.Models;

public sealed class User : Auditable
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string? Name { get; set; }
    public UserGender Gender { get; set; } = UserGender.Unspecified;
    public string? PhoneNumber { get; set; }

    // Navigation properties
    public ICollection<Role> Roles { get; set; } = [];
}
