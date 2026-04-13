using BuildingBlock.Pagination;
using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models;

namespace CineMeoTic.UserService.API.CQRS.Queries
{
    public record GetUsersQuery(PaginationRequest PaginationRequest) 
    : IQuery<GetUsersResult>;

    public record GetUsersResult(BuildingBlock.Pagination.PaginatedResult<GetUserResponse> Users);
}