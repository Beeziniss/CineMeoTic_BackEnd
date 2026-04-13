using BuildingBlock.Pagination;
using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.CQRS.Queries
{
    public class GetUsersHandler(IUserDbContext dbContext)
    : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        // get user with pagination
        // return result

        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await dbContext.Users.LongCountAsync(cancellationToken);

        var users = await dbContext.Users
                       .Skip(pageSize * pageIndex)
                       .Take(pageSize)
                       .ToListAsync(cancellationToken);

        return new GetUsersResult(
            new BuildingBlock.Pagination.PaginatedResult<GetUserResponse>(
                pageIndex,
                pageSize,
                totalCount,
                users.Select(u => new GetUserResponse(
                    Email: u.Email,
                    Password: u.PasswordHash
                ))));        
    }
}
}