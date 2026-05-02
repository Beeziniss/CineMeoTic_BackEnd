using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
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
            .Where(u => u.Id == userId)
            .ProjectToType<UserInfoResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestCustomException(MessageException.UserNotFound);

        IReadOnlyCollection<string> roleNames = await _userDbContext.UserRole
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

    private async Task CheckUserExistAsync(Guid userId, CancellationToken cancellationToken)
    {
        bool userExist = await _userDbContext.User
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExist)
        {
            throw new BadRequestCustomException(MessageException.UserNotFound);
        }
    }
    public async Task UpdateProfileAsync(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        Guid userId = _httpContextAccessor.GetUserId();

        await CheckUserExistAsync(userId, cancellationToken);

        await _userDbContext.User
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(updates =>
            {
                if(command.DisplayName is not null)
                {
                    updates.SetProperty(u => u.DisplayName, command.DisplayName);
                }

                if(command.Gender.HasValue)
                {
                    updates.SetProperty(u => u.Gender, command.Gender.Value);
                }

                if(command.PhoneNumber is not null)
                {
                    updates.SetProperty(u => u.PhoneNumber, command.PhoneNumber);
                }

                if(command.Avatar is not null)
                {
                    updates.SetProperty(u => u.Avatar, command.Avatar);
                }

                updates.SetProperty(u => u.UpdatedAt, CustomTimeProvider.GetUtcPlus7TimeOffset());
            }, cancellationToken);
    }
}
