using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class UserService(IUserDbContext userDbContext) : IUserService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private async Task CheckUserExistAsync(Guid userId, CancellationToken cancellationToken)
    {
        bool userExist = await _userDbContext.User
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExist)
        {
            throw new BadRequestCustomException(MessageException.UserNotFound);
        }
    }
    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        await CheckUserExistAsync(userId, cancellationToken);

        await _userDbContext.User
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
