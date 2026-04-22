using CineMeoTic.UserService.API.Models.Queries;
using CineMeoTic.UserService.API.Models.Commands;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IProfileService
{
    Task<UserInfoQueryResult> GetUserInfoAsync(CancellationToken cancellationToken);
    Task UpdateProfileAsync(UpdateUserProfileCommand command, CancellationToken cancellationToken);
}
