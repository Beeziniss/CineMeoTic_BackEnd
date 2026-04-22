using BuildingBlocks.Utils;
using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class PermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/permissions", async (CreatePermissionCommand permissionCommand, ISender sender) =>
            {
                await sender.Send(permissionCommand);

                return Results.Ok();
            }
            )
            .RequireRoles("Admin")
            .WithName("CreatePermission")
            .WithSummary("Create Permission")
            .WithDescription("Create Permission");

        app.MapPost("api/permissions/batch", async (CreatePermissionsCommand permissionsCommand, ISender sender) =>
            {
                await sender.Send(permissionsCommand);

                return Results.Ok();
            }
            )
            .RequireRoles("Admin")
            .WithName("CreatePermissions")
            .WithSummary("Create Permissions")
            .WithDescription("Create Multiple Permissions");
    }
}
