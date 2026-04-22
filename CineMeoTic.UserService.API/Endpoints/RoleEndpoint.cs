using BuildingBlocks.Utils;
using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class RoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/roles", async (CreateRoleCommand roleCommand, ISender sender) =>
            {
                await sender.Send(roleCommand);

                return Results.Ok();
            }
        )
        .RequireRoles("Admin")
        .WithName("CreateRole")
        .WithSummary("Create Role")
        .WithDescription("Create Role");
    }
}
