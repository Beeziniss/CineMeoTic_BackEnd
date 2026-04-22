using BuildingBlocks.Utils;
using Carter;
using CineMeoTic.UserService.API.Models.Commands;
using MediatR;

namespace CineMeoTic.UserService.API.Endpoints;

public sealed class AuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login", async (LoginCommand command, ISender sender) =>
            {
                LoginCommandResult result = await sender.Send(command);

                return Results.Ok(result);
            }
        )
        .WithName("LoginUser")
        .WithSummary("Login User")
        .WithDescription("Login User");

        app.MapPost("api/auth/register", async (RegisterCommand command, ISender sender) =>
           {
               await sender.Send(command);
               return Results.Ok();
           })
            .WithName("RegisterUser")
            .WithSummary("Register User")
            .WithDescription("Register User");

        app.MapPost("api/auth/refresh-token", async (ISender sender) =>
            {
                LoginCommandResult result = await sender.Send(new NoArgumentCommand());
                return Results.Ok(result);
            })
            .WithName("RefreshToken")
            .WithSummary("Refresh Access Token")
            .WithDescription("Refresh Access Token");

        app.MapPost("api/auth/change-password", async (ChangePasswordCommand command, ISender sender) =>
            {
                await sender.Send(command);
                return Results.Ok();
            })
            .RequireRoles("all")
            .WithName("ChangePassword")
            .WithSummary("Change Password")
            .WithDescription("Change Password");

        app.MapPost("api/auth/forgot-password", async (ForgotPasswordCommand command, ISender sender) =>
            {
                await sender.Send(command);
                return Results.Ok();
            })
            .WithName("ForgotPassword")
            .WithSummary("Forgot Password")
            .WithDescription("Forgot Password");

        app.MapPost("api/auth/reset-password", async (ResetPasswordCommand command, ISender sender) =>
            {
                await sender.Send(command);
                return Results.Ok();
            })
            .WithName("ResetPassword")
            .WithSummary("Reset Password")
            .WithDescription("Reset Password");
    }
}