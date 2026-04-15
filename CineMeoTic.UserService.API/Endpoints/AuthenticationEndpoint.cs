using Carter;
using CineMeoTic.UserService.API.Models.CQRS;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public class AuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (LoginCommand command, ISender sender) =>
            {
                LoginCommandResult result = await sender.Send(command);

                return Results.Ok(result);
            }
        )
        // .RequireAuthorization(policy => policy.RequireRole("User"))
        .WithName("LoginUser")
        .Produces<LoginCommandResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Login User")
        .WithDescription("Login User");

        app.MapPost("/register", async (RegisterCommand command, ISender sender) =>
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