using System.Runtime.Serialization;

namespace CineMeoTic.UserService.API.Data.Enums;

public enum Gender
{
    [EnumMember(Value = "Unspecified")]
    Unspecified = 0,
    [EnumMember(Value = "Male")]
    Male = 1,
    [EnumMember(Value = "Female")]
    Female = 2,
}
