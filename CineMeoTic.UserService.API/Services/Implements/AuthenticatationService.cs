using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Marten;
using System.Security.Claims;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IDocumentSession documentSession, IJsonWebTokenService jsonWebTokenService) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IDocumentSession _documentSession = documentSession;
    private readonly IJsonWebTokenService _jsonWebTokenService = jsonWebTokenService;

    private static async Task CheckEmailExistAsync(IDocumentSession documentSession, string email, CancellationToken cancellationToken)
    {
        string normalizeEmail = email.NormalizeLower();
        if (await documentSession.Query<User>().AnyAsync(u => u.Email.Equals(normalizeEmail, StringComparison.CurrentCultureIgnoreCase), cancellationToken))
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
        UserAuthInfoResponse? userAuthInfoResponse = await _documentSession.Query<User>()
            .ProjectToType<UserAuthInfoResponse>()
           .FirstOrDefaultAsync(u => u.Email == command.Email.NormalizeLower(), cancellationToken)
           ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        if (!VerifyPassword(command.Password, userAuthInfoResponse.PasswordHash))
        {
            throw new BadRequestCustomException(MessageException.InvalidCredential);
        }

        List<Claim> claims =
        [
            new Claim("sub", userAuthInfoResponse.Id.ToString()),
        ];

        IReadOnlyCollection<Guid> roleIds = await _documentSession.Query<UserRole>()
            .Where(ur => ur.UserId == userAuthInfoResponse.Id)
            .Select(ur => ur.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> roleNames = await _documentSession.Query<Role>()
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (string role in roleNames)
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
        await CheckEmailExistAsync(_documentSession, registerCommand.Email, cancellationToken);

        Role viewerRole = await _documentSession.Query<Role>().FirstOrDefaultAsync(r => r.Name == "Viewer", cancellationToken) ?? throw new NotFoundCustomException(MessageException.ViewerNotFound);

        User user = new()
        {
            Email = registerCommand.Email.NormalizeLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password),
            DisplayName = registerCommand.DisplayName,
            Gender = registerCommand.Gender,
        };

        UserRole userRole = new()
        {
            UserId = user.Id,
            RoleId = viewerRole.Id,
        };

        _documentSession.Store(user);
        _documentSession.Store(userRole);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return;
    }
}
