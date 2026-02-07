using CineMeoTic.UserService.API.Models;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IAuthenticationService
{
    Task RegisterAsync(RegisterRequest registerRequest);
    Task<AuthTokenResponse> Login(LoginRequest loginRequest);
}
