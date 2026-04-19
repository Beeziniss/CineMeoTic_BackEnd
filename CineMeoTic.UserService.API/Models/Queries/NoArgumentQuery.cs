using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Queries;

public sealed record NoArgumentQuery :
    IQuery<UserInfoQueryResult>
{
}
