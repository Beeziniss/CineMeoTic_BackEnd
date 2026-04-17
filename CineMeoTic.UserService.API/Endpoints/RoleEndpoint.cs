using Carter;
using CineMeoTic.UserService.API.Models.CQRS;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class RoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles", async (CreateRoleCommand roleCommand, ISender sender) =>
            {
                await sender.Send(roleCommand);

                return Results.Ok();
            }
        )
        .WithName("CreateRole")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Role")
        .WithDescription("Create Role");
    }
}
