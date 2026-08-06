using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.Auth.ChangePassword;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.NewPassword).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .MinimumLength(8).WithErrorCode(ErrorCodes.Validation.MinLength)
            .WithState(_ => ValidationErrorParams.Create(("min", 8)))
            .MaximumLength(64).WithErrorCode(ErrorCodes.Validation.MaxLength)
            .WithState(_ => ValidationErrorParams.Create(("max", 64)))
            .NotEqual(x => x.OldPassword).WithErrorCode(ErrorCodes.Auth.NewPasswordSameAsOld);
    }
}