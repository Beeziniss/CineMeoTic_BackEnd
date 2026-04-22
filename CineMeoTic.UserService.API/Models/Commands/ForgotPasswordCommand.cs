using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = null!;
}
