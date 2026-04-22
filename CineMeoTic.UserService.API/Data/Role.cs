using BuildingBlocks.Models;

namespace CineMeoTic.UserService.API.Data;

public sealed class Role : Auditable, IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
