using Cinemeotic.MovieService.API.Data;
using Cinemeotic.MovieService.API.Models;
using Cinemeotic.MovieService.API.Data.Enums;
using Mapster;

namespace CineMeoTic.MovieService.API.Mappings;
public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Movie, MovieResponse>()
              .Map(dest => dest.Genres,
                   src => src.Genres.Select(x => x.Name).ToList());

        config.NewConfig<Movie, MovieResponse>()
                .Map(dest => dest.Casters,
                     src => src.MovieCredits.Where(x => x.Role == Role.Caster).Select(x => x.Name).ToList());

        config.NewConfig<Movie, MovieResponse>()
            .Map(dest => dest.Directors,
                 src => src.MovieCredits.Where(x => x.Role == Role.Director).Select(x => x.Name).ToList());

        config.NewConfig<Movie, MovieResponse>()
            .Map(dest => dest.Rating,
                 src => src.MovieRatings == null
                     ? null
                     : src.MovieRatings.Select(x => x.Rating));
    }
}
