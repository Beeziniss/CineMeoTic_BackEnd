using System.Security.Claims;
using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Data.Enums;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using MapsterMapper;
using Marten;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IQuerySession session, IDocumentSession documentSession, IJsonWebTokenService jsonWebTokenService) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IQuerySession _session = session;
    private readonly IDocumentSession _documentSession = documentSession;
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
        UserInfoInternalResponse? user = await _session.Query<User>()
            .ProjectToType<UserInfoInternalResponse>()
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

        foreach (UserRole role in user.Roles.Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
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
        if (await IsEmailExistAsync(registerCommand.Email))
        {
            throw new BadRequestCustomException(MessageException.EmailAlreadyExists);
        }

        Role viewerRole = await _session.Query<Role>().FirstOrDefaultAsync(r => r.Name == UserRole.Viewer, cancellationToken)
            ?? throw new NotFoundCustomException("Viewer role not found.");

        User user = registerCommand.Adapt<User>();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password);
        user.Roles.Add(new Role { Name = UserRole.Viewer });

        _documentSession.Store(user);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return;
    }
}
