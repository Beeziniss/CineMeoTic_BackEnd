using CineMeoTic.Common.Models;

namespace CineMeoTic.UserService.API.Data;

public sealed class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
