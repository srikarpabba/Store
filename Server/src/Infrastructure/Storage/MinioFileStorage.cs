using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

internal sealed class MinioFileStorage(
    IAmazonS3 client,
    IOptions<MinioOptions> options)
    : IFileStorage
{
    private readonly IAmazonS3 _client = client;
    private readonly MinioOptions _options = options.Value;

    public Uri GetUrl(string objectKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddHours(1),
            Verb = HttpVerb.GET,
            Protocol = Protocol.HTTP

        };

        return new Uri(_client.GetPreSignedURL(request));
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
}
