using FluentValidation;
using MultiTenants.Boilerplate.Application.Queries;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Application.Validators;

/// <summary>
/// Validator for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessageConstants.RequiredField)
            .MaximumLength(ValidationLengthConstants.UserIdMaxLength)
            .WithMessage(ValidationMessageConstants.InvalidLength);
    }
}
