using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;

namespace Application.Brands.UploadBrandLogo;

public sealed record UploadBrandLogoCommand(Guid BrandId, FileUpload File) : ICommand;
