using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Utils;

public static class EndpointAuthorizationExtension
{
    public static RouteHandlerBuilder RequireRoles(this RouteHandlerBuilder builder, params string[] roles)
    {
        if (roles is null || roles.Length == 0)
        {
            throw new UnauthorizedCustomException(MessageException.RoleRequired);
        }

        return builder.RequireAuthorization(policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
            policy.RequireRole(roles);
        });
    }
}