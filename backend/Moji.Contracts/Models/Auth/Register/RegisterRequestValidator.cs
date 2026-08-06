using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .Matches(@"^[a-z0-9_]+$").WithErrorCode(ErrorCodes.Validation.InvalidFormat);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .MinimumLength(8).WithErrorCode(ErrorCodes.Validation.MinLength)
            .WithState(_ => ValidationErrorParams.Create(("min", 8)))
            .MaximumLength(64).WithErrorCode(ErrorCodes.Validation.MaxLength)
            .WithState(_ => ValidationErrorParams.Create(("max", 64)));
        RuleFor(x => x.Email).EmailAddress().WithErrorCode(ErrorCodes.Validation.InvalidEmail);
        RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .MinimumLength(1).WithErrorCode(ErrorCodes.Validation.MinLength)
            .WithState(_ => ValidationErrorParams.Create(("min", 1)));
        RuleFor(x => x.LastName).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required).MinimumLength(1)
            .WithErrorCode(ErrorCodes.Validation.MinLength)
            .WithState(_ => ValidationErrorParams.Create(("min", 1)));
    }
}