using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Services.Intefaces;
using MapsterMapper;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IMapper mapper) : IAuthenticationService
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IMapper mapper = mapper;

    #region Helper methods
    private async Task<bool> IsEmailExistAsync(string email)
    {
        string normalizeEmail = email.NormalizeLower();
        return false;
    }
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    #endregion

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        if (await IsEmailExistAsync(registerRequest.Email))
        {
            return;
        }

        User newUser = mapper.Map<RegisterRequest, User>(registerRequest);

        return;
    }

    public async Task<AuthTokenResponse> Login(LoginRequest loginRequest)
    {
        Role role = new()
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
        };

        User user = mapper.Map<LoginRequest, User>(loginRequest);

        if (user.Roles.Any(x => x.Name != "Admin"))
        {
            throw new UnauthorizedAccessException("User does not have the required role.");
        }

        return new AuthTokenResponse
        {
            AccessToken = string.Empty,
            RefreshToken = string.Empty
        };
    }


}
