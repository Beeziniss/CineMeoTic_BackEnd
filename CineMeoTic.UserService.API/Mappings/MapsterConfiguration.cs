using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;
using Mapster;

namespace CineMeoTic.UserService.API.Mappings;

public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region Model to DB
        config.NewConfig<RegisterRequest, User>()
              .Map(dest => dest.PasswordHash,
                   src => BCrypt.Net.BCrypt.HashPassword(src.Password));

        config.NewConfig<LoginRequest, User>()
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
