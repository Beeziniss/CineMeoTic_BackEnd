using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record ChangePasswordCommand : ICommand
{
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
}
