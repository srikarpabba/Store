namespace Infrastructure.Storage;

public sealed class MinioOptions
{
    public const string SectionName = "Minio";

    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// Endpoint browsers can reach, used for presigned URLs. Inside Docker
    /// the API talks to MinIO via the compose hostname, which the browser
    /// cannot resolve — presign against this instead. Falls back to
    /// <see cref="Endpoint"/> when empty.
    /// </summary>
    public string PublicEndpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public string PublicBaseUrl { get; init; } = string.Empty;
    public bool UseSsl { get; init; }
}
