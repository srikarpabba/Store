namespace Application.Abstractions.Storage;

public sealed record FileUpload(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);
