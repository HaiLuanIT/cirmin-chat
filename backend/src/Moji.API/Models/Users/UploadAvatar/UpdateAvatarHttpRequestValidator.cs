using FluentValidation;

namespace Moji.API.Models.Users.UploadAvatar;

public class UpdateAvatarHttpRequestValidator : AbstractValidator<UpdateAvatarHttpRequest>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public UpdateAvatarHttpRequestValidator()
    {
        RuleFor(request => request.Image).NotNull().WithMessage("Ảnh đại diện là bắt buộc.");
        When(request => request.Image is not null, () =>
        {
            RuleFor(request => request.Image!.Length)
                .GreaterThan(0)
                .WithMessage("Ảnh không được rỗng.")
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithMessage("Ảnh không được vượt quá 5 mb.");

            RuleFor(request => request.Image!.ContentType)
                .Must(AllowedContentType.Contains)
                .WithMessage("Chỉ hỗ trợ JPEG, PNG and WebP.");
        });
    }
}