using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS;

public sealed class RoleHandler(IRoleService roleService) :
    IRequestHandler<CreateRoleCommand, Unit>
{
    private readonly IRoleService _roleService = roleService;

    public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        await _roleService.CreateRoleAsync(request, cancellationToken);
        return Unit.Value;
    }
}
