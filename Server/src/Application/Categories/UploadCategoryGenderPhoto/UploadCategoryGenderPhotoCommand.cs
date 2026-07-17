using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;

namespace Application.Categories.UploadCategoryGenderPhoto;

public sealed record UploadCategoryGenderPhotoCommand(Guid CategoryId, Guid GenderId, FileUpload File) : ICommand;
