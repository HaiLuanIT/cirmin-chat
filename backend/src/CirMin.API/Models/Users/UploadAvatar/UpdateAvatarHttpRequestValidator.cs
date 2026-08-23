using FluentValidation;
using CirMin.Contracts.Errors;

namespace CirMin.API.Models.Users.UploadAvatar;

public class UpdateAvatarHttpRequestValidator : AbstractValidator<UpdateAvatarHttpRequest>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
    private const int MaxFileSizeInMb = 5;

    private static readonly HashSet<string> AllowedContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private static readonly string[] AllowedFormats =
    [
        "JPEG",
        "PNG",
        "WebP"
    ];

    public UpdateAvatarHttpRequestValidator()
    {
        RuleFor(request => request.Image).NotNull().WithErrorCode(ErrorCodes.Media.ImageRequired);
        When(request => request.Image is not null, () =>
        {
            RuleFor(request => request.Image!.Length)
                .GreaterThan(0)
                .WithErrorCode(ErrorCodes.Media.EmptyImage)
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithErrorCode(ErrorCodes.Media.ImageTooLarge)
                .WithState(_ => ValidationErrorParams.Create(("max", MaxFileSizeInMb)));

            RuleFor(request => request.Image!.ContentType)
                .Must(AllowedContentType.Contains)
                .WithErrorCode(ErrorCodes.Media.UnsupportedImageType)
                .WithState(_ => ValidationErrorParams.Create(("formats", AllowedFormats)));
        });
    }
}