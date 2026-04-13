namespace CineMeoTic.UserService.API.Models
{
    public sealed record CreateUserRequest
    (
        Guid Id,
        String Username,
        String Password
    );
}