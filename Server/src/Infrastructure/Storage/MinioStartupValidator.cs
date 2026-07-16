using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

internal sealed class MinioStartupValidator(
    IAmazonS3 client,
    IOptions<MinioOptions> options)
    : IHostedService
{
    private readonly IAmazonS3 _client = client;
    private readonly MinioOptions _options = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _client.GetBucketLocationAsync(
                new GetBucketLocationRequest
                {
                    BucketName = _options.BucketName
                },
                cancellationToken);
        }
        catch (AmazonS3Exception ex) when (
            ex.ErrorCode == "NoSuchBucket" ||
            ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException(
                $"MinIO bucket '{_options.BucketName}' does not exist.",
                ex);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
