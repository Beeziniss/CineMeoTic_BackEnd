using Carter;
using CineMeoTic.UserService.API.Models.Queries;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class ProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profile/me", async (ISender sender) =>
            {
                UserInfoQueryResult result = await sender.Send(new NoArgumentQuery());
                return Results.Ok(result);
            }
        )
        .RequireAuthorization("all")
        .WithName("GetUserProfile")
        .WithSummary("Get User Profile")
        .WithDescription("Get User Profile");
    }
}
