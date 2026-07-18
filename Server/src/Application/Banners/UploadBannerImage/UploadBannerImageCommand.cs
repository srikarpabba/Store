using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;

namespace Application.Banners.UploadBannerImage;

public sealed record UploadBannerImageCommand(Guid BannerId, FileUpload File) : ICommand;
