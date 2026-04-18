using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.CQRS;

public sealed record CreatePermissionsCommand : ICommand
{
    public IEnumerable<string> Names { get; set; } = null!;
}
