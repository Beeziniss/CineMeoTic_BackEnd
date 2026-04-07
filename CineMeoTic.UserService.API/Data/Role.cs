using CineMeoTic.Common.Models;

namespace CineMeoTic.UserService.API.Data;

public sealed class Role : Auditable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<User>? Users { get; set; }
    public ICollection<Permission>? Permissions { get; set; }
}
