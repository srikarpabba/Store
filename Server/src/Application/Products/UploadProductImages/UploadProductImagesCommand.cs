using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;

namespace Application.Products.UploadProductImages;

public sealed record UploadProductImagesCommand(
    Guid ProductId,
    Guid ProductColorId,
    IReadOnlyCollection<FileUpload> Files)
    : ICommand;
