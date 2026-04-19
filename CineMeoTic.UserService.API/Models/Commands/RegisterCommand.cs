using BuildingBlocks.CQRS;
using BuildingBlocks.Behaviors;
using CineMeoTic.UserService.API.Data.Enums;
using System.Text.Json.Serialization;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record RegisterCommand : ICommand
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    [JsonConverter(typeof(EnumMemberJsonConverter<Gender>))]
    public Gender Gender { get; set; } = Gender.Unspecified;
}