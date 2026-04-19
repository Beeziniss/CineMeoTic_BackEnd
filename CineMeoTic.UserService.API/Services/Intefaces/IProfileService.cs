using CineMeoTic.UserService.API.Models.Queries;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IProfileService
{
    Task<UserInfoQueryResult> GetUserInfoAsync(CancellationToken cancellationToken);
}
