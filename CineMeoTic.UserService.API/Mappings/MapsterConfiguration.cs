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
        config.NewConfig<RegisterCommand, User>()
              .Map(dest => dest.PasswordHash,
                   src => BCrypt.Net.BCrypt.HashPassword(src.Password));

        config.NewConfig<LoginCommand, User>()
            .Map(dest => dest.PasswordHash,
                 src => BCrypt.Net.BCrypt.HashPassword(src.Password));
        #endregion

        #region DB to Model
        config.NewConfig<User, UserInfoInternalResponse>()
              .Map(dest => dest.Roles,
                   src => src.Roles.Select(r => r.Name));
        #endregion
    }
}
