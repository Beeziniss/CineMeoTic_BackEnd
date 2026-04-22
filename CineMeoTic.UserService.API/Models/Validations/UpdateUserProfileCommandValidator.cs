using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .MinimumLength(2).WithMessage(MessageException.DisplayNameMinLength)
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName))
            .MaximumLength(100).WithMessage(MessageException.DisplayNameMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage(MessageException.InvalidGenderFormat)
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.PhoneNumber)
            .Matches(RegexPattern.PhoneNumberRegexPattern)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Avatar)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .When(x => !string.IsNullOrWhiteSpace(x.Avatar))
            .WithMessage(MessageException.InvalidAvatarFormat);

        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage(MessageException.FieldRequired);

    }
    private bool HaveAtLeastOneField(UpdateUserProfileCommand x)
    {
        return
            !string.IsNullOrWhiteSpace(x.DisplayName) ||
            x.Gender.HasValue ||
            !string.IsNullOrWhiteSpace(x.PhoneNumber) ||
            !string.IsNullOrWhiteSpace(x.Avatar);
    }
}