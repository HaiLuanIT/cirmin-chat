using Moji.BusinessLogic.Services.Storage;

namespace Moji.API.IntegrationTests.Infrastructure;

public class FakeImageStorageService : IImageStorageService
{
    public Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName, string folderName,
        CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Image storage must not be called by update-user-info test");
    }

    public Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Image storage must not be called by update-user-info test");
    }
}