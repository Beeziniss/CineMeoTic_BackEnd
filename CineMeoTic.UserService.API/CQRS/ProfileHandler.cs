using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS;

public sealed class ProfileHandler(IProfileService profileService) :
    IQueryHandler<NoArgumentQuery, UserInfoQueryResult>,
    ICommandHandler<UpdateUserProfileCommand, Unit>
{
    private readonly IProfileService _profileService = profileService;

    public async Task<UserInfoQueryResult> Handle(NoArgumentQuery query, CancellationToken cancellationToken)
    {
        return await _profileService.GetUserInfoAsync(cancellationToken);
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        await _profileService.UpdateProfileAsync(command, cancellationToken);
        return Unit.Value;
    }
}
