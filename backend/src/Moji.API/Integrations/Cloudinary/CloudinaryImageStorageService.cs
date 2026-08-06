using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Services.Storage;
using Moji.Contracts.Errors;
using ImageUploadResult = Moji.BusinessLogic.Services.Storage.ImageUploadResult;

namespace Moji.API.Integrations.Cloudinary;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly CloudinaryOptions _cloudinaryOptions;

    public CloudinaryImageStorageService(IOptions<CloudinaryOptions> cloudinaryOptions)
    {
        _cloudinaryOptions = cloudinaryOptions.Value;
        var acc = new Account(_cloudinaryOptions.CloudName, _cloudinaryOptions.ApiKey, _cloudinaryOptions.ApiSecret);
        _cloudinary = new CloudinaryDotNet.Cloudinary(acc);

        if (_cloudinary == null) throw new InvalidOperationException("Cloudinary is not initialized");
    }

    public async Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName, string folderName,
        CancellationToken cancellationToken)
    {
        // validate file
        if (imageStream == null) throw new ArgumentNullException(nameof(imageStream), "Image cannot be null or empty");

        // init publicId, folder, image upload params
        var publicId = Guid.NewGuid().ToString();

        var uploadParam = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            AssetFolder = folderName,
            PublicId = publicId,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = true
        };


        try
        {
            var uploadResult = await _cloudinary.UploadAsync(uploadParam, cancellationToken);
            if (uploadResult == null)
                throw new MediaStorageException(ErrorCodes.Media.StorageUnavailable,
                    new InvalidOperationException("Cloudinary upload result is null"));

            if (uploadResult.Error is not null)
                throw new MediaStorageException(ErrorCodes.Media.StorageUnavailable,
                    new InvalidOperationException($"Cloudinary upload error: {uploadResult.Error.Message}"));

            // map to ImageUploadResult and return

            return new ImageUploadResult(uploadResult.PublicId, uploadResult.SecureUrl.ToString(), uploadResult.Width,
                uploadResult.Height, uploadResult.Bytes, uploadResult.Format);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (MediaStorageException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new MediaStorageException(ErrorCodes.Media.StorageUnavailable, e);
        }
    }

    public async Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken)
    {
        if (publicId == null) return false;
        try
        {
            var deleteParams = new DeletionParams(publicId)
            {
                Invalidate = true
            };
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
            if (deleteResult.Result != "ok") return false;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Delete image error: {e.Message}");
            return false;
        }

        return true;
    }
}