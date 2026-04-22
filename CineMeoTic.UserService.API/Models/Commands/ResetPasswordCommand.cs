using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record ResetPasswordCommand : ICommand
{
    public string Otp { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
}
