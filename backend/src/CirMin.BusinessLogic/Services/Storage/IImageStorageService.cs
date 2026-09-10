namespace CirMin.BusinessLogic.Services.Storage;

public interface IImageStorageService
{
    Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName, string folderName,
        CancellationToken cancellationToken);

    Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken);
}

public sealed record ImageUploadResult(
    string PublicId,
    string SecureUrl,
    int Width,
    int Height,
    long Bytes,
    string Format);