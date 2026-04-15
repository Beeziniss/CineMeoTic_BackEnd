using System.Security.Claims;
using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Marten;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IQuerySession session, IJsonWebTokenService jsonWebTokenService) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IQuerySession _session = session;
    private readonly IJsonWebTokenService _jsonWebTokenService = jsonWebTokenService;

#region Helper methods
    private async Task<bool> IsEmailExistAsync(string email)
    {
        string normalizeEmail = email.NormalizeLower();
        return await _session.Query<User>().AnyAsync(u => u.Email.Equals(normalizeEmail, StringComparison.CurrentCultureIgnoreCase));
    }
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    #endregion

    public async Task<LoginCommandResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        if (await IsEmailExistAsync(command.Email))
        {
            throw new BadRequestCustomException(MessageException.EmailAlreadyExists);
        }

        User? user = await _session.Query<User>()
            .FirstOrDefaultAsync(u => u.Email == command.Email.NormalizeLower(), cancellationToken)
            ?? throw new NotFoundCustomException(MessageException.UserNotFound);

        var result = user.Adapt<UserInfoInternalResponse>();

        if (!VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestCustomException(MessageException.InvalidCredential);
        }

        IEnumerable<Claim> claims =
        [
            new Claim("Sub", user.Id.ToString()),
        ];

        CookieOptions cookieOptions = new()
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            MaxAge = TimeSpan.FromDays(7)
        };

        string token = _jsonWebTokenService.GenerateAccessToken(claims);
        string refreshToken = _jsonWebTokenService.GenerateRefreshToken();

        //todo: luu refresh token vao redis

        if (command.IsRememberMe)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, cookieOptions);
        }

        return new LoginCommandResult(token, refreshToken);
    }

    public Task RegisterAsync(RegisterRequest registerRequest)
    {
        throw new NotImplementedException();
    }
}
