using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class AuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/login", async (LoginCommand command, ISender sender) =>
            {
                LoginCommandResult result = await sender.Send(command);

                return Results.Ok(result);
            }
        )
        .WithName("LoginUser")
        .WithSummary("Login User")
        .WithDescription("Login User");

        app.MapPost("api/register", async (RegisterCommand command, ISender sender) =>
           {
               await sender.Send(command);
               return Results.Ok();
           }
           )
            .WithName("RegisterUser")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Register User")
            .WithDescription("Register User");
    }
}