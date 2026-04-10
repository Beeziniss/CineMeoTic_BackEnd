using CineMeoTic.Common.Models;
using CineMeoTic.UserService.API.Data.Enums;

namespace CineMeoTic.UserService.API.Data;

public sealed class Role : Auditable
{
    public Guid Id { get; set; }
    public UserRole Name { get; set; }

    public ICollection<User> Users { get; } = [];
    public ICollection<Permission> Permissions { get; } = [];
}
