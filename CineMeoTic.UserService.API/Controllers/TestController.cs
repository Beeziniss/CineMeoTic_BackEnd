using Carter;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Services;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CineMeoTic.UserService.API.Controllers
{
    public class TestController : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/user", async (CreateUserRequest create, [FromServices] IDocumentSession session) =>
                {
                    var user = new UserModelTest
                    {
                        Username = create.Username,
                        Password = create.Password
                    };
                    session.Store(user);

                    // Commit all outstanding changes in one
                    // database transaction
                    await session.SaveChangesAsync();
                }
            );

            app.MapGet("/users", async (bool internalOnly, [FromServices] IDocumentSession session, CancellationToken ct) =>
                {
                    return await session.Query<UserModelTest>()
                                        .ToListAsync(ct);
                }
            ).RequireAuthorization(policy => policy.RequireRole("User"))
             .Produces<List<UserModelTest>>()
             .ProducesProblem(StatusCodes.Status400BadRequest);

            // OR use the lightweight IQuerySession if all you're doing is running queries
            app.MapGet("/user/{id:guid}", async (Guid id, [FromServices] IQuerySession session, CancellationToken ct) =>
                {
                    return await session.LoadAsync<UserModelTest>(id, ct);
                }
            );

            app.MapPost("/login", async (LoginTestRequest login, [FromServices] IJsonWebTokenService jwtService, [FromServices] IDocumentSession session) =>
            {
                var user = await session.Query<UserModelTest>()
                                         .FirstOrDefaultAsync(x => x.Username == login.Username && x.Password == login.Password);
                if (user == null)
                {
                    return Results.NotFound();
                }
                Claim[] claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "User")
                };

                var token = jwtService.GenerateAccessToken(claims, 1);
                return Results.Ok(new { Token = token });
            });
        }


    }
}
