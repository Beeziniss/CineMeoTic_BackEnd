using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using CineMeoTic.UserService.API.Models.CQRS;
using Mapster;

namespace CineMeoTic.UserService.API.Mappings;

public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region Model to DB
        #endregion

        #region DB to Model
        #endregion
    }
}
