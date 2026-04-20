using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class ProfileService(IUserDbContext userDbContext, IHttpContextAccessor httpContextAccessor) : IProfileService
{
    private readonly IUserDbContext _userDbContext = userDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<UserInfoQueryResult> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        Guid userId = _httpContextAccessor.GetUserId();

        UserInfoResponse userInfoResponse = await _userDbContext.User
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectToType<UserInfoResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        IReadOnlyCollection<string> roleNames = await _userDbContext.UserRole
            .AsNoTracking()
            .Where(ur => ur.UserId == userInfoResponse.Id)
            .Join(
                _userDbContext.Role.AsNoTracking(),
                ur => ur.RoleId,
                r => r.Id,
                (_, r) => r.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        UserInfoQueryResult userInfoQueryResult = userInfoResponse.Adapt<UserInfoQueryResult>() with
        {
            Roles = roleNames
        };

        return userInfoQueryResult;
    }
}
