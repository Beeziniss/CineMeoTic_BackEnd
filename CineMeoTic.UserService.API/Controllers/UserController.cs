//using BuildingBlock.Pagination;
//using Carter;
//using CineMeoTic.UserService.API.CQRS.Commands.CreateUser;
//using CineMeoTic.UserService.API.CQRS.Queries;
//using CineMeoTic.UserService.API.Models;
//using Mapster;
//using MediatR;

//namespace CineMeoTic.UserService.API.Controllers
//{
//    public class UserController : ICarterModule
//    {

//        public record GetUsersResponse(BuildingBlock.Pagination.PaginatedResult<GetUserResponse> Users);

//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapGet("/users", async ([AsParameters] PaginationRequest request, ISender sender) =>
//            {
//                GetUsersResult result = await sender.Send(new GetUsersQuery(request));

//                GetUsersResponse response = result.Adapt<GetUsersResponse>();

//                return Results.Ok(response);
//            })
//            .WithName("GetUsers")
//            .Produces<GetUsersResponse>(StatusCodes.Status200OK)
//            .ProducesProblem(StatusCodes.Status400BadRequest)
//            .ProducesProblem(StatusCodes.Status404NotFound)
//            .WithSummary("Get Users")
//            .WithDescription("Get Users");

//            app.MapPost("/users", async (CreateUserRequest request, ISender sender) => 
//                {
//                    var command = request.Adapt<CreateUserCommand>();
//                    var result = await sender.Send(command);
//                    var response = result.Adapt<CreateUserResponse>();

//                    return Results.Created($"/users/{response.Id}",response);
//                }
//            )
//            // .RequireAuthorization(policy => policy.RequireRole("User"))
//            .WithName("CreateUser")
//            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
//            .ProducesProblem(StatusCodes.Status400BadRequest)
//            .WithSummary("Create User")
//            .WithDescription("Create User");
//        }

//        //viet tiep update vs delete here
//    }
//}