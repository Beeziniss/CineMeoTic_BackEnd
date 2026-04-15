using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.CQRS;

public sealed record LoginCommand : ICommand<LoginCommandResult>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool IsRememberMe { get; init; } = false;
}
