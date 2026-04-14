using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models;

public sealed record class LoginRequest : IQuery<string>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsRememberMe { get; set; } = false;
}
