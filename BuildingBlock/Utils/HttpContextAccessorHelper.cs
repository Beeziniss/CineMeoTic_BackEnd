using BuildingBlocks.Exceptions.Handler;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Utils;

public static class HttpContextAccessorHelper
{
    public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        Guid userId = Guid.Parse(httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new UnAuthenticatedCustomException(MessageException.UnAuthenticated));

        Console.WriteLine($"User ID: {userId}");

        return userId;
    }
}
