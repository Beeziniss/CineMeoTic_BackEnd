using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class AuthenticatationService(IHttpContextAccessor httpContextAccessor, IUserDbContext userDbContext, IJsonWebTokenService jsonWebTokenService, IEmailService emailService, IRedisCacheService redisCacheService, IBackgroundJobQueue backgroundJobQueue) : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserDbContext _userDbContext = userDbContext;
    private readonly IJsonWebTokenService _jsonWebTokenService = jsonWebTokenService;
    private readonly IEmailService _emailService = emailService;
    private readonly IRedisCacheService _redisCacheService = redisCacheService;
    private readonly IBackgroundJobQueue _backgroundJobQueue = backgroundJobQueue;

    private static async Task CheckEmailExistAsync(IUserDbContext userDbContext, string email, CancellationToken cancellationToken)
    {
        string normalizeEmail = email.NormalizeLower();
        if (await userDbContext.User.AsNoTracking().AnyAsync(u => u.Email == normalizeEmail, cancellationToken))
        {
            throw new BadRequestCustomException(MessageException.EmailAlreadyExists);
        }
    }
    private static void VerifyPassword(string password, string passwordHash)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
        {
            throw new BadRequestCustomException(MessageException.InvalidCredential);
        }
    }
    public async Task<LoginCommandResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        string normalizedEmail = command.Email.NormalizeLower();

        UserAuthInfoResponse userAuthInfoResponse = await _userDbContext.User
            .Where(u => u.Email == normalizedEmail)
            .ProjectToType<UserAuthInfoResponse>()
            .FirstOrDefaultAsync(cancellationToken)
           ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        VerifyPassword(command.Password, userAuthInfoResponse.PasswordHash);

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userAuthInfoResponse.Id.ToString()),
        ];

        IReadOnlyCollection<Guid> roleIds = await _userDbContext.UserRole
            .Where(ur => ur.UserId == userAuthInfoResponse.Id)
            .Select(ur => ur.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> roleNames = await _userDbContext.Role
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

        CookieOptions cookieOptions = new()
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        };

        if (command.IsRememberMe)
        {
            cookieOptions.MaxAge = TimeSpan.FromDays(7);
        }

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        await _redisCacheService.SetStringAsync($"refresh_token:{userAuthInfoResponse.Id}", refreshToken, TimeSpan.FromDays(30));

        return new LoginCommandResult
        {
            AccessToken = token
        };
    }

    public async Task RegisterAsync(RegisterCommand registerCommand, CancellationToken cancellationToken)
    {
        await CheckEmailExistAsync(_userDbContext, registerCommand.Email, cancellationToken);

        Role viewerRole = await _userDbContext.Role.FirstOrDefaultAsync(r => r.Name == "Viewer", cancellationToken) ?? throw new NotFoundCustomException(MessageException.ViewerNotFound);

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

        await _userDbContext.ExecuteInTransactionAsync(async () =>
        {
            _userDbContext.User.Add(user);
            _userDbContext.UserRole.Add(userRole);

            await _userDbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return;
    }

    public async Task<LoginCommandResult> RefreshTokenAsync()
    {
        string refreshTokenFromCookie = _httpContextAccessor.GetRefreshToken();
        string? refreshTokenFromRedis = await _redisCacheService.GetStringAsync(refreshTokenFromCookie);

        if (string.IsNullOrEmpty(refreshTokenFromRedis) || refreshTokenFromRedis != refreshTokenFromCookie)
        {
            throw new UnauthorizedCustomException(MessageException.InvalidRefreshToken);
        }

        Guid userId = _httpContextAccessor.GetUserId();
        IEnumerable<Claim> payload = _httpContextAccessor.GetPayload();
        string accessToken = _jsonWebTokenService.GenerateAccessToken(payload);
        string refreshToken = _jsonWebTokenService.GenerateRefreshToken();

        await _redisCacheService.SetStringAsync($"refresh_token:{userId}", refreshToken, TimeSpan.FromDays(30));

        return new LoginCommandResult
        {
            AccessToken = accessToken
        };
    }

    public async Task ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        Guid userId = _httpContextAccessor.GetUserId();
        string passwordHash = await _userDbContext.User
            .Where(u => u.Id == userId)
            .Select(x => x.PasswordHash)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundCustomException(MessageException.UserNotFound);

        VerifyPassword(command.CurrentPassword, passwordHash);
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);

        await _userDbContext.User
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(x => x.PasswordHash, newPasswordHash)
                .SetProperty(x => x.UpdatedAt, CustomTimeProvider.GetUtcPlus7TimeOffset())
                , cancellationToken);
    }

    private static async Task<string> GenerateOtp()
    {
        const string digits = "0123456789";
        Random random = new();
        StringBuilder otp = new(6);

        for (int i = 0; i < 6; i++)
        {
            otp.Append(digits[random.Next(digits.Length)]);
        }

        return otp.ToString();
    }
    public async Task ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        string normalizeEmail = command.Email.NormalizeLower();

        string otp = await GenerateOtp();
        string otpRedisKey = $"forgot_password_otp:{normalizeEmail}";

        await _redisCacheService.SetStringAsync(otpRedisKey, otp, TimeSpan.FromMinutes(10));

        _backgroundJobQueue.Enqueue(async ct =>
        {
            _emailService.Send(EmailTemplateType.ForgotPassword, normalizeEmail, otp);
            await Task.CompletedTask;
        });
    }
    private async Task VerifyOtpAsync(string normalizeEmail, string otp)
    {

        string otpFromRedis = await _redisCacheService.GetStringAsync($"forgot_password_otp:{normalizeEmail}") ?? throw new BadRequestCustomException(MessageException.InvalidOtp);

        if (!string.Equals(otpFromRedis, otp, StringComparison.Ordinal))
        {
            throw new BadRequestCustomException(MessageException.InvalidOtp);
        }
    }
    public async Task ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        string normalizeEmail = command.Email.NormalizeLower();
        await VerifyOtpAsync(normalizeEmail, command.Otp);

        User user = await _userDbContext.User
            .FirstOrDefaultAsync(u => u.Email == normalizeEmail, cancellationToken)
            ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);

        await _userDbContext.User
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(x => x.PasswordHash, newPasswordHash)
                .SetProperty(x => x.UpdatedAt, CustomTimeProvider.GetUtcPlus7TimeOffset())
                , cancellationToken);
    }
}
