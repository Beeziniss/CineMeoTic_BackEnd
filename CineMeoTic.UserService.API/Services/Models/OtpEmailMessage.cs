namespace CineMeoTic.UserService.API.Services.Models;

public sealed record OtpEmailMessage
{
    public string Email { get; init; } = null!;
    public string Otp { get; init; } = null!;
}
