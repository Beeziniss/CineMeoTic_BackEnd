using Carter;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.CQRS;
using Mapster;
using MediatR;

namespace CineMeoTic.UserService.API.Controllers
{
   public class LoginEndpoint : ICarterModule
   {

       public void AddRoutes(IEndpointRouteBuilder app)
       {
           app.MapPost("/login", async (LoginRequest request, ISender sender) => 
               {
                   var command = request.Adapt<LoginCommand>();
                   var result = await sender.Send(command);
                   var response = result.Adapt<AuthTokenResponse>();

                   return Results.Ok(response);
               }
           )
           // .RequireAuthorization(policy => policy.RequireRole("User"))
           .WithName("LoginUser")
           .Produces<CreateUserResponse>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .WithSummary("Login User")
           .WithDescription("Login User");
       }
   }
}