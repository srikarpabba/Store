namespace Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> UploadAsync(FileUpload file, string objectName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
    Uri GetUrl(string objectKey);
}
