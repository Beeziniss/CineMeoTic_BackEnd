using System.Runtime.Serialization;

namespace Cinemeotic.MovieService.API.Data.Enums;

public enum Role
{
    [EnumMember(Value = "Caster")]
    Caster = 0,
    [EnumMember(Value = "Director")]
    Director = 1
}
