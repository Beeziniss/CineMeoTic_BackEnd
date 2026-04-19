using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class PermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/permissions", async (CreatePermissionCommand permissionCommand, ISender sender) =>
            {
                await sender.Send(permissionCommand);

                return Results.Ok();
            }
            )
            .WithName("CreatePermission")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Permission")
            .WithDescription("Create Permission");

        app.MapPost("/permissions/batch", async (CreatePermissionsCommand permissionsCommand, ISender sender) =>
            {
                await sender.Send(permissionsCommand);

                return Results.Ok();
            }
            )
            .WithName("CreatePermissions")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Permissions")
            .WithDescription("Create Multiple Permissions");
    }
}
