using BuildingBlocks.Exceptions;
using BuildingBlocks.Exceptions.Handler;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Utils;

public static class HttpContextAccessorExtension
{
    public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        Guid userId = Guid.Parse(httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new UnAuthenticatedCustomException(MessageException.UnAuthenticated));

        return userId;
    }

    public static IEnumerable<Claim> GetPayload(this IHttpContextAccessor httpContextAccessor)
    {
        IEnumerable<Claim> payload = httpContextAccessor.HttpContext?.User.Claims ?? throw new UnAuthenticatedCustomException(MessageException.UnAuthenticated);

        return payload;
    }

    public static string GetRefreshToken(this IHttpContextAccessor httpContextAccessor)
    {
        string refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"] ?? throw new BadRequestCustomException(MessageException.RefreshTokenNotFound);

        return refreshToken;
    }
}
