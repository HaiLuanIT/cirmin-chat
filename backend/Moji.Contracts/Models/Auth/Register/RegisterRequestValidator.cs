using FluentValidation;

namespace Moji.Contracts.Models.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username không được bỏ trống!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password không được bỏ trống!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email không hợp lệ!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name không được để trống")
            .MinimumLength(1).WithMessage("First name phải có ít nhất 1 ký tự");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name không được để trống").MinimumLength(1)
            .WithMessage("Last name phải có ít nhất 1 ký tự");
    }
}