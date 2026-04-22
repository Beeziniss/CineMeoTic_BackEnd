using BuildingBlocks.Behaviors;
using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Data.Enums;
using System.Text.Json.Serialization;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record UpdateUserProfileCommand : ICommand
{
    public string? DisplayName { get; init; }
    [JsonConverter(typeof(NullableEnumMemberJsonConverter<Gender>))]
    public Gender? Gender { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Avatar { get; init; }
}