using BuildingBlocks.Utils;
using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class UserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/users/{userId:guid}", async (Guid userId, ISender sender) =>
        {
            await sender.Send(new DeleteUserCommand { UserId = userId });
            return Results.Ok();
        }
        )
        .RequireRoles("Admin")
        .WithName("DeleteUserByAdmin")
        .WithSummary("Delete User By Admin")
        .WithDescription("Delete User By Admin");
    }
}
