using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Marten;

namespace CineMeoTic.UserService.API.UserApi.Authentication
{
    public sealed class AuthenticationHandler (IAuthenticationService authenticationService) : ICommandHandler<LoginCommand, LoginCommandResult>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<LoginCommandResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            return await _authenticationService.LoginAsync(command, cancellationToken);
        }
    }
}