using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IUserDbContext userDbContext, IJsonWebTokenService jsonWebTokenService) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserDbContext _userDbContext = userDbContext;
    private readonly IJsonWebTokenService _jsonWebTokenService = jsonWebTokenService;

    private static async Task CheckEmailExistAsync(IUserDbContext userDbContext, string email, CancellationToken cancellationToken)
    {
        string normalizeEmail = email.NormalizeLower();
        if (await userDbContext.User.AsNoTracking().AnyAsync(u => u.Email == normalizeEmail, cancellationToken))
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
        string normalizedEmail = command.Email.NormalizeLower();

        UserAuthInfoResponse userAuthInfoResponse = await _userDbContext.User
            .AsNoTracking()
            .Where(u => u.Email == normalizedEmail)
            .ProjectToType<UserAuthInfoResponse>()
            .FirstOrDefaultAsync(cancellationToken)
           ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        if (!VerifyPassword(command.Password, userAuthInfoResponse.PasswordHash))
        {
            throw new BadRequestCustomException(MessageException.InvalidCredential);
        }

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userAuthInfoResponse.Id.ToString()),
        ];

        IReadOnlyCollection<Guid> roleIds = await _userDbContext.UserRole
            .AsNoTracking()
            .Where(ur => ur.UserId == userAuthInfoResponse.Id)
            .Select(ur => ur.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> roleNames = await _userDbContext.Role
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (string role in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        string token = _jsonWebTokenService.GenerateAccessToken(claims);
        string refreshToken = _jsonWebTokenService.GenerateRefreshToken();

        // TODO: Save refresh token to Redis

        CookieOptions cookieOptions = new()
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None
        };

        if (command.IsRememberMe)
        {
            cookieOptions.MaxAge = TimeSpan.FromDays(7);
        }

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, cookieOptions);

        return new LoginCommandResult
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    public async Task RegisterAsync(RegisterCommand registerCommand, CancellationToken cancellationToken)
    {
        await CheckEmailExistAsync(_userDbContext, registerCommand.Email, cancellationToken);

        Role viewerRole = await _userDbContext.Role.AsNoTracking().FirstOrDefaultAsync(r => r.Name == "Viewer", cancellationToken) ?? throw new NotFoundCustomException(MessageException.ViewerNotFound);

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = registerCommand.Email.NormalizeLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password),
            DisplayName = registerCommand.DisplayName,
            Gender = registerCommand.Gender,
            Avatar = "https://img-9gag-fun.9cache.com/photo/a0eyj2v_460s.jpg"
        };

        UserRole userRole = new()
        {
            UserId = user.Id,
            RoleId = viewerRole.Id,
        };

        _userDbContext.User.Add(user);
        _userDbContext.UserRole.Add(userRole);

        await _userDbContext.SaveChangesAsync(cancellationToken);

        return;
    }
}
