using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IUserDbContext userDbContext, IJsonWebTokenService jsonWebTokenService) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserDbContext _userDbContext = userDbContext;
    private readonly IJsonWebTokenService _jsonWebTokenService = jsonWebTokenService;

    private async Task CheckEmailExistAsync(string email)
    {
        string normalizeEmail = email.NormalizeLower();
        if (await _userDbContext.User.AnyAsync(u => u.Email.Equals(normalizeEmail, StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new BadRequestCustomException(MessageException.EmailAlreadyExists);
        }
    }
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    public async Task<LoginCommandResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await _userDbContext.User
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == command.Email.NormalizeLower(), cancellationToken)
            ?? throw new NotFoundCustomException(MessageException.UserNotFound);

        if (!VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestCustomException(MessageException.InvalidCredential);
        }

        List<Claim> claims =
        [
            new Claim("sub", user.Id.ToString()),
        ];

        foreach (string role in user.Roles.Select(r => r.Name).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        CookieOptions cookieOptions = new()
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            MaxAge = TimeSpan.FromDays(7)
        };

        string token = _jsonWebTokenService.GenerateAccessToken(claims);
        string refreshToken = _jsonWebTokenService.GenerateRefreshToken();

        // TODO: Save refresh token to Redis

        if (command.IsRememberMe)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, cookieOptions);
        }

        return new LoginCommandResult
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    public async Task RegisterAsync(RegisterCommand registerCommand, CancellationToken cancellationToken)
    {
        await CheckEmailExistAsync(registerCommand.Email);

        Role viewerRole = await _userDbContext.Role.FirstOrDefaultAsync(r => r.Name == "Viewer", cancellationToken)
            ?? throw new NotFoundCustomException("Viewer role not found.");

        User user = new()
        {
            Email = registerCommand.Email.NormalizeLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password),
            DisplayName = registerCommand.DisplayName,
            Gender = registerCommand.Gender,
            Avatar = string.Empty,
        };
        user.Roles.Add(viewerRole);

        _userDbContext.User.Add(user);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return;
    }
}
