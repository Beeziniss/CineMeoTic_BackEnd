using CineMeoTic.Common.Models;

namespace CineMeoTic.UserService.API.Data;

public sealed class RolePermission : Auditable
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
