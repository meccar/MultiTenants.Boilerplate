using FluentValidation;
using MultiTenants.Boilerplate.Application.Commands;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Application.Validators;

/// <summary>
/// Validator for CreateUserCommand
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessageConstants.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationMessageConstants.InvalidEmail)
            .MaximumLength(ValidationLengthConstants.EmailMaxLength)
            .WithMessage(ValidationMessageConstants.InvalidLength);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(ValidationMessageConstants.RequiredField)
            .MinimumLength(ValidationLengthConstants.UserNameMinLength)
            .WithMessage(ValidationMessageConstants.InvalidLength)
            .MaximumLength(ValidationLengthConstants.UserNameMaxLength)
            .WithMessage(ValidationMessageConstants.InvalidLength)
            .Matches(RegexConstants.UserName)
            .WithMessage(ValidationMessageConstants.InvalidFormat);

        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage(ValidationMessageConstants.RequiredField)
            .MinimumLength(ValidationLengthConstants.PasswordMinLength)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage(ValidationMessageConstants.InvalidLength)
            .Matches(RegexConstants.UppercaseLetter)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage(ValidationMessageConstants.InvalidFormat)
            .Matches(RegexConstants.LowercaseLetter)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage(ValidationMessageConstants.InvalidFormat)
            .Matches(RegexConstants.Digit)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage(ValidationMessageConstants.InvalidFormat);
    }
}
