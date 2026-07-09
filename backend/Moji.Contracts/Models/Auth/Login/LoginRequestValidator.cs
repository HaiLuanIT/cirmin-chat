using FluentValidation;

namespace Moji.Contracts.Models.Auth.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username không được để trống!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password không được để trống!");
    }
}