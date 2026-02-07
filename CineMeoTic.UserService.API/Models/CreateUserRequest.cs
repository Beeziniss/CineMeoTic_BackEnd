namespace CineMeoTic.UserService.API.Models
{
    public sealed record CreateUserRequest
    (
        string Username,
        string Password
    );
}
