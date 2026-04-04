using FluentValidation;
using BuildingBlocks.Application.Queries.GetCurrentUser;
using BuildingBlocks.Shared.Constants;

namespace BuildingBlocks.Application.Validators;

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
