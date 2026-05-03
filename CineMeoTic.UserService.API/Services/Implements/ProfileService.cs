using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Services.Intefaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        User user = new()
        {
            Id = userId
        };
        _userDbContext.User.Attach(user);

        EntityEntry<User> entry = _userDbContext.Entry(user);

        if (command.DisplayName is not null)
        {
            user.DisplayName = command.DisplayName;
            entry.Property(u => u.DisplayName).IsModified = true;
        }

        if (command.Gender.HasValue)
        {
            user.Gender = command.Gender.Value;
            entry.Property(u => u.Gender).IsModified = true;
        }

        if (command.PhoneNumber is not null)
        {
            user.PhoneNumber = command.PhoneNumber;
            entry.Property(u => u.PhoneNumber).IsModified = true;
        }

        if (command.Avatar is not null)
        {
            user.Avatar = command.Avatar;
            entry.Property(u => u.Avatar).IsModified = true;
        }

        user.UpdatedAt = CustomTimeProvider.GetUtcPlus7TimeOffset();
        entry.Property(u => u.UpdatedAt).IsModified = true;
    }
}
