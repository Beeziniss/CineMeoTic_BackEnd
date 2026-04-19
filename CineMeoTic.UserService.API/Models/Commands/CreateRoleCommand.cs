using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record CreateRoleCommand : ICommand
{
    public string RoleName { get; init; } = null!;
    public IEnumerable<string> PermissionNames { get; init; } = null!;
}
