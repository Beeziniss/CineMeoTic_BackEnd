using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Services;
using Mapster;
using Marten;
using System.Security.Claims;

namespace CineMeoTic.UserService.API.CQRS.Commands;

public sealed class LoginHandler(IUserDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IJsonWebTokenService jsonWebToken) : IQueryHandler<LoginRequest, string>
{
    private readonly IUserDbContext dbContext = dbContext;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IJsonWebTokenService jsonWebToken = jsonWebToken;

    #region Helper methods
    private async Task<bool> IsEmailExistAsync(string email)
    {
        string normalizeEmail = email.NormalizeLower();
        bool userEmail = await dbContext.Users
            .Where(u => u.Email.NormalizeLower() == normalizeEmail)
            .Select(u => u.Email)
            .AnyAsync();

        return userEmail;
    }
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    #endregion

    public async Task<string> Handle(LoginRequest query, CancellationToken cancellationToken)
    {
        if (await IsEmailExistAsync(query.Email))
        {
            throw new BadRequestCustomException(MessageException.EmailAlreadyExists);
        }

        UserInfoInternalResponse? user = await dbContext.Users
            .Where(u => u.Email == query.Email.NormalizeLower())
            .ProjectToType<UserInfoInternalResponse>()
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundCustomException(MessageException.UserNotFound);

        if (!VerifyPassword(query.Password, user.PasswordHash))
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

        string token = jsonWebToken.GenerateAccessToken(claims);

        if (query.IsRememberMe)
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, cookieOptions);
        }

        return token;
    }
}
