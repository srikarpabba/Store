namespace Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> UploadAsync(FileUpload file, string objectName, CancellationToken cancellationToken = default);
    Uri GetUrl(string objectKey);
}
