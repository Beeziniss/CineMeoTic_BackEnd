using CineMeoTic.Common.Utils;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Helpers;

public static class HttpContextAccessorHelper
{
    public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        Guid userId = Guid.Parse(httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? throw new UnAuthenticatedCustomException(MessageException.UnAuthenticated));

        return userId;
    }
}
