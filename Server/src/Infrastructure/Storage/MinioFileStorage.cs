using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

internal sealed class MinioFileStorage(
    IAmazonS3 client,
    [FromKeyedServices(MinioFileStorage.PresignClientKey)] IAmazonS3 presignClient,
    IOptions<MinioOptions> options)
    : IFileStorage
{
    public const string PresignClientKey = "minio-presign";

    private readonly IAmazonS3 _client = client;
    private readonly IAmazonS3 _presignClient = presignClient;
    private readonly MinioOptions _options = options.Value;

    public Uri GetUrl(string objectKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);

        bool publicIsHttps = _options.PublicEndpoint.StartsWith(
            "https://",
            StringComparison.OrdinalIgnoreCase);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddHours(1),
            Verb = HttpVerb.GET,
            Protocol = publicIsHttps ? Protocol.HTTPS : Protocol.HTTP
        };

        return new Uri(_presignClient.GetPreSignedURL(request));
    }

    public async Task<string> UploadAsync(FileUpload file, string objectName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectName);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            InputStream = file.Content,
            ContentType = file.ContentType,
            AutoCloseStream = false
        };

        await _client.PutObjectAsync(request, cancellationToken);

        return objectName;
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);

        await _client.DeleteObjectAsync(
            new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey
            },
            cancellationToken);
    }
}
