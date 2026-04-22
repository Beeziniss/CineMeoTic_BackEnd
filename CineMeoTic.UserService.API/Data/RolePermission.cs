namespace CineMeoTic.UserService.API.Data;

public sealed class RolePermission : IEntity
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
