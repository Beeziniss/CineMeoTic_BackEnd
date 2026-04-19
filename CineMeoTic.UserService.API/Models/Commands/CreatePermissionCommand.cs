using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record CreatePermissionCommand : ICommand
{
    public string Name { get; init; } = null!;
}
