using Application.Abstractions.Messaging;

namespace Application.Categories.DeleteCategoryGenderPhoto;

public sealed record DeleteCategoryGenderPhotoCommand(Guid CategoryId, Guid GenderId) : ICommand;
