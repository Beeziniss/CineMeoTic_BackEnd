using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS.Commands;

public sealed class AuthenticationHandler(IAuthenticationService authenticationService) :
    ICommandHandler<RegisterCommand, Unit>,
    ICommandHandler<LoginCommand, LoginCommandResult>
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
}