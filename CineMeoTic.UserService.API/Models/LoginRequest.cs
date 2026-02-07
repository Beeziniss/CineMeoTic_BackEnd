namespace CineMeoTic.UserService.API.Models;

public sealed record class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsRememberMe { get; set; } = false;
}
