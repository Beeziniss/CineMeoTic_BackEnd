using CineMeoTic.Common.Models;

namespace CineMeoTic.UserService.API.Data;

public sealed class Permission : Auditable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Role> Roles { get; set; } = [];
}
