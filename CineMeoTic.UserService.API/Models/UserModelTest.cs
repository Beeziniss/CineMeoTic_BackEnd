namespace CineMeoTic.UserService.API.Models;

public class UserModelTest
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
