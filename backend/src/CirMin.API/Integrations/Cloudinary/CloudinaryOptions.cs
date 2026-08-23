namespace CirMin.API.Integrations.Cloudinary;

public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public required string CloudName { get; init; }

    public required string ApiKey { get; init; }

    public required string ApiSecret { get; init; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(CloudName)) throw new ArgumentException("CloudName is required");
        if (string.IsNullOrEmpty(ApiKey)) throw new ArgumentException("ApiKey is required");
        if (string.IsNullOrEmpty(ApiSecret)) throw new ArgumentException("ApiSecret is required");
    }
}