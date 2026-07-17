using Application.Abstractions.Messaging;

namespace Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
