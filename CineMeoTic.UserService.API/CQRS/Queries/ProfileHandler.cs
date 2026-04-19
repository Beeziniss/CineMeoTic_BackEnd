using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Services.Intefaces;

namespace CineMeoTic.UserService.API.CQRS.Queries;

public sealed class ProfileHandler(IProfileService profileService) :
    IQueryHandler<NoArgumentQuery, UserInfoQueryResult>
{
    private readonly IProfileService _profileService = profileService;

    public async Task<UserInfoQueryResult> Handle(NoArgumentQuery query, CancellationToken cancellationToken)
    {
        return await _profileService.GetUserInfoAsync(cancellationToken);
    }
}
