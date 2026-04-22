using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IAuthenticationService
{
    Task ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken);
    Task ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken);
    Task<LoginCommandResult> LoginAsync(LoginCommand loginCommand, CancellationToken cancellationToken);
    Task<LoginCommandResult> RefreshTokenAsync();
    Task RegisterAsync(RegisterCommand registerCommand, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
}
