using FluentValidation;

namespace Moji.Contracts.Models.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username không được bỏ trống!")
            .Matches(@"^[a-z0-9_]+$").WithMessage("Username chỉ được chứa chữ thường, số và dấu gạch nối");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password không được bỏ trống!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email không hợp lệ!");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name không được để trống")
            .MinimumLength(1).WithMessage("First name phải có ít nhất 1 ký tự");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name không được để trống").MinimumLength(1)
            .WithMessage("Last name phải có ít nhất 1 ký tự");
    }
}