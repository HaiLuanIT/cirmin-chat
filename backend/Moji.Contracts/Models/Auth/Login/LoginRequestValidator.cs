using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.Auth.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
    }
}