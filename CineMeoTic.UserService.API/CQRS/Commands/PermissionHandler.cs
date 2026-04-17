using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS.Commands;

public sealed class PermissionHandler(IPermissionService permissionService) : IRequestHandler<CreatePermissionCommand, Unit>
{
    private readonly IPermissionService _permissionService = permissionService;

    public async Task<Unit> Handle(CreatePermissionCommand createPermissionCommand, CancellationToken cancellationToken)
    {
        await _permissionService.CreatePermissionAsync(createPermissionCommand, cancellationToken);

        return Unit.Value;
    }
}