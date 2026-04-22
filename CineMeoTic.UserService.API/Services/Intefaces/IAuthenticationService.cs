using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IAuthenticationService
{
    Task<LoginCommandResult> LoginAsync(LoginCommand loginCommand, CancellationToken cancellationToken);
    Task<LoginCommandResult> RefreshTokenAsync();
    Task RegisterAsync(RegisterCommand registerCommand, CancellationToken cancellationToken);
}
