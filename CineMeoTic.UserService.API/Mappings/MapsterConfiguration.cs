using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Models.Queries;
using Mapster;

namespace CineMeoTic.UserService.API.Mappings;

public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region Model to DB
        #endregion

        #region DB to Model
        config.NewConfig<User, UserInfoQueryResult>()
            .Ignore(dest => dest.Roles);
        #endregion
    }
}
