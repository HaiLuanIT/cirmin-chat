using CirMin.BusinessLogic.Exceptions;

namespace CirMin.API.Helpers;

public static class ImageValidatorHelper
{
    private static readonly byte[] PngSignature =
    [
        0x89, 0x50, 0x4E, 0x47,
        0x0D, 0x0A, 0x1A, 0x0A
    ];

    private static readonly byte[] JpegSignature =
    [
        0xFF,
        0xD8,
        0xFF
    ];

    private static readonly HashSet<string> AllowedFormats =
    [
        "PNG",
        "JPEG",
        "WEBP"
    ];

    public static async Task IsSupportedImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();
        var isValidHeader = await ValidateImageHeader(stream);
        if (!isValidHeader) throw new CirMinBadRequestException("Định dạng file ảnh không được hỗ trợ.");
        // try
        // {
        //     var imageInfo = await Image.IdentifyAsync(stream, cancellationToken);
        //
        //     if (imageInfo == null) throw new ArgumentException("Nội dung file ảnh không được để trống");
        //
        //     var format = imageInfo.Metadata.DecodedImageFormat;
        //     if (format == null || !AllowedFormats.Contains(format.Name))
        //         throw new ArgumentException("Chỉ hỗ trợ ảnh PNG, JPEG, WEBP");
        // }
        // catch (UnknownImageFormatException)
        // {
        //     throw new ArgumentException("Nội dung file không phải định dạng ảnh được hỗ trợ.");
        // }
        // catch (InvalidImageContentException)
        // {
        //     throw new ArgumentException("File ảnh bị hỏng hoặc nội dung không hợp lệ.");
        // }
    }

    private static async Task<bool> ValidateImageHeader(Stream stream)
    {
        Memory<byte> header = new byte[12];

        var bytesRead = await stream.ReadAsync(header);

        if (stream.CanSeek) stream.Position = 0;

        ReadOnlySpan<byte> span = header.Span;
        if (bytesRead < 12) return false;

        if (span[..8].SequenceEqual(PngSignature)) return true;

        if (span[..3].SequenceEqual(JpegSignature)) return true;

        if (span[..4].SequenceEqual("RIFF"u8) && span.Slice(8, 4).SequenceEqual("WEBP"u8)) return true;

        return false;
    }
}