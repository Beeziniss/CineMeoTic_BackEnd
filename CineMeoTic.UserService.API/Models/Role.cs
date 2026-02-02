using CineMeoTic.Common.Models;

namespace CineMeoTic.UserService.API.Models;

public sealed class Role : Auditable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation properties
    public ICollection<User> Users { get; set; } = [];
    public ICollection<Permission> Permissions { get; set; } = [];
}
