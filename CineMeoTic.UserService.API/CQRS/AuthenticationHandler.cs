using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS;

public sealed class AuthenticationHandler(IAuthenticationService authenticationService) :
    ICommandHandler<RegisterCommand, Unit>,
    ICommandHandler<LoginCommand, LoginCommandResult>,
    ICommandHandler<ForgotPasswordCommand, Unit>,
    ICommandHandler<ResetPasswordCommand, Unit>,
    ICommandHandler<ChangePasswordCommand, Unit>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Unit> Handle(RegisterCommand registerCommand, CancellationToken cancellationToken)
    {
        await _authenticationService.RegisterAsync(registerCommand, cancellationToken);
        return Unit.Value;
    }

    public async Task<LoginCommandResult> Handle(LoginCommand loginCommand, CancellationToken cancellationToken)
    {
        return await _authenticationService.LoginAsync(loginCommand, cancellationToken);
    }

    public async Task<LoginCommandResult> Handle(NoArgumentCommand _)
    {
        return await _authenticationService.RefreshTokenAsync();
    }

    public async Task<Unit> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await _authenticationService.ChangePasswordAsync(command, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        await _authenticationService.ForgotPasswordAsync(command, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await _authenticationService.ResetPasswordAsync(command, cancellationToken);
        return Unit.Value;
    }
}