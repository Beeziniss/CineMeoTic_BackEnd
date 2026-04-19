using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Marten;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class ProfileService(IDocumentSession documentSession, IHttpContextAccessor httpContextAccessor) : IProfileService
{
    private readonly IDocumentSession _documentSession = documentSession;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<UserInfoQueryResult> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        Guid userId = _httpContextAccessor.GetUserId();

        UserInfoResponse userInfoResponse = await _documentSession.Query<User>()
            .Where(u => u.Id == userId)
            .ProjectToType<UserInfoResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        IReadOnlyCollection<string> roleNames = await _documentSession.Query<Role>()
            .Where(r => _documentSession.Query<UserRole>()
                .Where(ur => ur.UserId == userInfoResponse.Id)
                .Select(ur => ur.RoleId)
                .Contains(r.Id))
            .Select(r => r.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        UserInfoQueryResult userInfoQueryResult = userInfoResponse.Adapt<UserInfoQueryResult>() with
        {
            Roles = roleNames
        };

        return userInfoQueryResult;
    }
}
