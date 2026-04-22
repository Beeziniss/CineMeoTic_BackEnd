namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IUserService
{
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
