using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.CQRS;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IAuthenticationService
{
    Task RegisterAsync(RegisterRequest registerRequest);
    Task<LoginCommandResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
}
